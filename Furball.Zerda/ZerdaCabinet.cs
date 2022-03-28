using System.IO.MemoryMappedFiles;
using System.Text;

namespace Furball.Zerda;

public class ZerdaCabinet {
    private List<Entry> _entries;
    private long        _fileOffset;
    private FileStream  _fileStream;

    public ZerdaCabinet(string filepath) {
        this._entries = new List<Entry>();

        FileStream fileStream = File.OpenRead(filepath);
        BinaryReader fileReader = new BinaryReader(fileStream, Encoding.Default, true);

        //File Magic
        if (new string(fileReader.ReadChars(3)) != "ZRD")
            throw new InvalidDataException("File Magic invalid.");

        long entryCount = fileReader.ReadInt64();

        for (int i = 0; i != entryCount; i++) {
            this._entries.Add(Entry.Read(fileReader));
        }

        this._fileOffset = fileReader.BaseStream.Position;
        this._fileStream = fileStream;
    }

    ~ZerdaCabinet() {
        this._fileStream.Close();
    }

    public ZerdaCabinet() {
        this._entries = new List<Entry>();
    }

    public void AddFile(string filepath, string desiredName = "") {
        byte[] fileBytes = File.ReadAllBytes(filepath);

        if (desiredName == "")
            desiredName = Path.GetFileName(filepath);

        Entry newEntry = new Entry {
            Filename = desiredName,
            Location = 0,
            ModificationDate = DateTimeOffset.Now.ToUnixTimeSeconds(),
            Size = fileBytes.Length,
            FileData = fileBytes
        };

        this._entries.Add(newEntry);
    }

    public void Write(string filepath) {
        using FileStream fileStream = File.OpenWrite(filepath);
        using BinaryWriter fileWriter = new BinaryWriter(fileStream, Encoding.Default, true);

        byte[] files = this.WriteFiles();
        byte[] entryListing = this.WriteHeaderAndEntryListing();

        fileWriter.Write(entryListing);
        fileWriter.Write(files);
    }

    private byte[] WriteHeaderAndEntryListing() {
        MemoryStream stream = new MemoryStream();
        using BinaryWriter fileWriter = new BinaryWriter(stream, Encoding.Default, true);

        fileWriter.Write(new char[] { 'Z', 'R', 'D' });

        fileWriter.Write((long) this._entries.Count);

        for (int i = 0; i < this._entries.Count; i++) {
            Entry currentEntry = this._entries[i];

            currentEntry.WriteListing(fileWriter);
        }

        return stream.ToArray();
    }

    private byte[] WriteFiles() {
        MemoryStream stream = new MemoryStream();
        using BinaryWriter fileWriter = new BinaryWriter(stream, Encoding.Default, true);

        for (int i = 0; i < this._entries.Count; i++) {
            Entry currentEntry = this._entries[i];
            currentEntry.Location = fileWriter.BaseStream.Position;

            fileWriter.Write(currentEntry.FileData);
        }

        return stream.ToArray();
    }

    public byte[] ReadFile(string filename) {
        Entry foundEntry = this._entries.Find(file => file.Filename == filename);

        byte[] readData = new byte[foundEntry.Size];

        this._fileStream.Seek(foundEntry.Location + this._fileOffset, SeekOrigin.Begin);
        int read = this._fileStream.Read(readData, 0, (int) foundEntry.Size);

        if (read != foundEntry.Size)
            throw new InvalidDataException("Read less than expected, file is likely corrupt.");

        return readData;
    }
}

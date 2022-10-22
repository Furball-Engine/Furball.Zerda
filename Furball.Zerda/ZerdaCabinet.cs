using System.Text;
using SubstreamSharp;

namespace Furball.Zerda;

public class ZerdaCabinet {
	/// <summary>
	///     The offset where files start in the cabinet
	/// </summary>
	private long _fileOffset;

	/// <summary>
	///     The stream where we read the cabinet data from
	/// </summary>
	private FileStream _stream;

	/// <summary>
	///     The entries contained in this cabinet
	/// </summary>
	public List<Entry> Entries { get; private set; }

	/// <summary>
	///     The filename of the cabinet
	/// </summary>
	public string Filename;

	public ZerdaCabinet(string filepath) {
		FileStream fileStream = File.OpenRead(filepath);

		this.LoadFromStream(fileStream);
	}

	public ZerdaCabinet() {
		this.Entries = new List<Entry>();
	}

	private void LoadFromStream(FileStream stream) {
		if (!stream.CanSeek)
			throw new ArgumentException("Stream must be seekable");

		if (!stream.CanRead)
			throw new ArgumentException("Stream must be readable");

		BinaryReader fileReader = new BinaryReader(stream, Encoding.UTF8, true);

		//File Magic
		if (new string(fileReader.ReadChars(3)) != "ZRD")
			throw new InvalidDataException("File Magic invalid.");

		//Read the amount of entries
		long entryCount = fileReader.ReadInt64();

		//Create a new list with a preset capacity to avoid resizing the list multiple times while adding entries
		this.Entries = new List<Entry>((int)entryCount);

		//Read all the entries one after another
		for (int i = 0; i != entryCount; i++)
			this.Entries.Add(Entry.Read(fileReader));

		//Save the file offset for later use
		this._fileOffset = fileReader.BaseStream.Position;
		this._stream     = stream;
	}

	~ZerdaCabinet() {
		this._stream.Close();
	}

	/// <summary>
	///     Adds a new file to the cabinet
	/// </summary>
	/// <param name="filepath">The path to the file</param>
	/// <param name="desiredName">The desired name in the cabinet</param>
	public void AddFile(string filepath, string desiredName = "") {
		//Read all the bytes of the file
		byte[] fileBytes = File.ReadAllBytes(filepath);

		//If the desired name is empty, use the filename of the file
		if (desiredName == "")
			desiredName = Path.GetFileName(filepath);

		this.AddFile(fileBytes, desiredName);
	}

	public void AddFile(byte[] data, string desiredName = "") {
		//If the desired name is empty, use a random name
		if (desiredName == "")
			desiredName = Guid.NewGuid().ToString();

		//Create a new entry with the desired name and the file bytes
		Entry newEntry = new Entry {
			Filename         = desiredName,
			Location         = 0,
			ModificationDate = DateTimeOffset.Now.ToUnixTimeSeconds(),
			Size             = data.Length,
			FileData         = data
		};

		//Add the entry to the list
		this.Entries.Add(newEntry);	
	}

	/// <summary>
	///     Writes the cabinet to a file
	/// </summary>
	/// <param name="filepath">The output file</param>
	public void Write(string filepath) {
		using FileStream   fileStream = File.OpenWrite(filepath);
		using BinaryWriter fileWriter = new BinaryWriter(fileStream, Encoding.UTF8, true);

		this.WriteFiles(fileWriter);
		this.WriteHeaderAndEntryListing(fileWriter);

		//Make sure to flush the contents to the file
		fileWriter.Flush();
	}

	/// <summary>
	///     Writes the header and entry listing to a byte array
	/// </summary>
	private void WriteHeaderAndEntryListing(BinaryWriter writer) {
		//Write the file magic
		writer.Write(new[] { 'Z', 'R', 'D' });

		//Write the amount of entries
		writer.Write((long)this.Entries.Count);

		//Write all the entries
		foreach (Entry currentEntry in this.Entries)
			currentEntry.WriteListing(writer);
	}

	/// <summary>
	///     Write the files to a byte array
	/// </summary>
	private void WriteFiles(BinaryWriter writer) {
		//Write all the files
		foreach (Entry currentEntry in this.Entries) {
			currentEntry.Location = writer.BaseStream.Position;

			writer.Write(currentEntry.FileData);
		}
	}
	
	/// <summary>
	/// Gets a substream of the main file stream
	/// </summary>
	/// <param name="filename">The name of the file in the cabinet</param>
	/// <returns>A substream of the file stream containing the file</returns>
	/// <exception cref="FileNotFoundException"></exception>
	public Substream GetFile(string filename) {
		Entry entry = this.Entries.Find(x => x.Filename == filename);

		if (entry == null)
			throw new FileNotFoundException("File not found in cabinet");

		return new Substream(this._stream, this._fileOffset + entry.Location, entry.Size);
	}

	/// <summary>
	///     Read a file from the cabinet
	/// </summary>
	/// <param name="filename">The name of the file</param>
	/// <returns>The read data</returns>
	/// <exception cref="InvalidDataException">The read data was invalid</exception>
	public byte[] ReadFile(string filename) {
		//Find the entry with the given filename
		Entry foundEntry = this.Entries.Find(file => file.Filename == filename);

		//If the entry was not found, throw an exception
		if (foundEntry == null)
			throw new InvalidDataException("File not found in cabinet");

		//Create a buffer to store the read data
		byte[] readData = new byte[foundEntry.Size];

		//Seek to the location of the file
		this._stream.Seek(foundEntry.Location + this._fileOffset, SeekOrigin.Begin);
		//Read the file data
		int read = this._stream.Read(readData, 0, (int)foundEntry.Size);

		//If the amount of read bytes is not equal to the size of the file, throw an exception
		if (read != foundEntry.Size)
			throw new InvalidDataException("Read less than expected, file is likely corrupt.");

		return readData;
	}
}

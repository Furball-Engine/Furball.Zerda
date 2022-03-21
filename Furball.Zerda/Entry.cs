namespace Furball.Zerda;

public class Entry {
    public long  Location;
    public long   Size;
    public long   ModificationDate;
    public string Filename;

    public byte[] FileData;

    public static Entry Read(BinaryReader reader) {
        Entry entry = new Entry {
            Location         = reader.ReadInt64(),
            Size             = reader.ReadInt64(),
            ModificationDate = reader.ReadInt64(),
            Filename         = reader.ReadString()
        };

        return entry;
    }

    public void WriteListing(BinaryWriter writer) {
        writer.Write(this.Location);
        writer.Write(this.Size);
        writer.Write(this.ModificationDate);
        writer.Write(this.Filename);
    }
}

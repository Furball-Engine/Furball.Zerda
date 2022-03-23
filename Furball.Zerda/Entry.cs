namespace Furball.Zerda;

public class Entry {
    public EntryType EntryType;
    public long      Location;
    public long      Size;
    public long      ModificationDate;
    public string    Filename;

    public byte[] FileData;

    public static Entry Read(BinaryReader reader) {
        Entry entry = new Entry {
            EntryType        = (EntryType) reader.ReadInt32(),
            Location         = reader.ReadInt64(),
            Size             = reader.ReadInt64(),
            ModificationDate = reader.ReadInt64(),
            Filename         = reader.ReadString()
        };

        return entry;
    }

    public void WriteListing(BinaryWriter writer) {
        writer.Write((int) this.EntryType);
        writer.Write(this.Location);
        writer.Write(this.Size);
        writer.Write(this.ModificationDate);
        writer.Write(this.Filename);
    }
}

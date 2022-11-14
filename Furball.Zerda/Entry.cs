namespace Furball.Zerda;

public class Entry {
	public byte[] FileData;
	public string Filename;
	public long   Location;
	public long   ModificationDate;
	public long   Size;

	/// <summary>
	/// Read an entry from a stream
	/// </summary>
	/// <param name="reader">The reader to read from</param>
	/// <returns>The read entry</returns>
	public static Entry Read(BinaryReader reader) {
		Entry entry = new Entry {
			Location         = reader.ReadInt64(),
			Size             = reader.ReadInt64(),
			ModificationDate = reader.ReadInt64(),
			Filename         = reader.ReadString()
		};

		return entry;
	}

	/// <summary>
	/// Writes the entry to a stream
	/// </summary>
	/// <param name="writer">The writer to write to</param>
	public void WriteListing(BinaryWriter writer) {
		writer.Write(this.Location);
		writer.Write(this.Size);
		writer.Write(this.ModificationDate);
		writer.Write(this.Filename);
	}
}

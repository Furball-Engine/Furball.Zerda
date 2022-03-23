namespace Furball.Zerda.EntryTypes;

public abstract class Serializable {
    public abstract void ReadFromStream(Entry entryDetails, Stream stream);
    public abstract void WriteToStream (Entry entryDetails, Stream stream);
}

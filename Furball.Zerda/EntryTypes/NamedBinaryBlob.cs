namespace Furball.Zerda.EntryTypes;

public class NamedBinaryBlob : Serializable {
    public byte[] Data;

    public override void ReadFromStream(Entry entryDetails, Stream stream) {
        if (stream.Read(this.Data, 0, (int)entryDetails.Size) != entryDetails.Size)
            throw new InvalidDataException("Size did not match read size, file is likely corrupt.");
    }

    public override void WriteToStream(Entry entryDetails, Stream stream) {

    }
}

namespace Minerva;

public class RawObject
{
    private readonly ObjectType type;

    private readonly int length;

    private readonly byte[] data;

    public RawObject(ObjectType type, int length, Span<byte> data)
    {
        this.type = type;
        this.length = length;
        this.data = data.ToArray();
    }

    public ObjectType Type => type;

    public int Length => length;

    public ReadOnlySpan<byte> Data => data;
}

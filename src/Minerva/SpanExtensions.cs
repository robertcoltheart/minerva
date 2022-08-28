namespace Minerva;

public static class SpanExtensions
{
    public static Span<byte> SliceLine(this Span<byte> value)
    {
        var index = value.IndexOf((byte)'\n');

        return index < 0
            ? value
            : value[..index];
    }
}

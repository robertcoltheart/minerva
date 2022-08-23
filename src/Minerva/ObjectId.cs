using System.Buffers.Binary;

namespace Minerva;

public readonly struct ObjectId : IEquatable<ObjectId>
{
    private static readonly uint[] HexLookup = new uint[256];

    private readonly byte[] value;

    static ObjectId()
    {
        for (var i = 0; i < 256; i++)
        {
            var s = i.ToString("x2");
            HexLookup[i] = Convert.ToUInt32(s[0]) + (Convert.ToUInt32(s[1]) << 16);
        }
    }

    public ObjectId(byte[] value)
    {
        if (value.Length != 20)
        {
            throw new ArgumentException("Invalid SHA value", nameof(value));
        }

        this.value = value;
    }

    public ReadOnlySpan<byte> RawId => value;

    public bool Equals(ObjectId other)
    {
        return RawId.SequenceEqual(other.RawId);
    }

    public override bool Equals(object obj)
    {
        return obj is ObjectId objectId && Equals(objectId);
    }

    public override int GetHashCode()
    {
        return BinaryPrimitives.ReadInt32LittleEndian(RawId[..4]);
    }

    public override string ToString()
    {
        var chars = new char[40];

        TryWrite(chars, 0, 20, out _);

        return new string(chars);
    }

    public bool TryWritePrefix(Span<char> destination, out int charsWritten)
    {
        return TryWrite(destination, 0, 1, out charsWritten);
    }

    public bool TryWriteId(Span<char> destination, out int charsWritten)
    {
        return TryWrite(destination, 1, 19, out charsWritten);
    }

    private bool TryWrite(Span<char> destination, int index, int count, out int charsWritten)
    {
        if (destination.Length < count * 2)
        {
            charsWritten = 0;

            return false;
        }

        for (var i = 0; i < count; i++)
        {
            var val = HexLookup[RawId[i + index]];

            destination[i * 2] = (char) val;
            destination[i * 2 + 1] = (char) (val >> 16);
        }

        charsWritten = count * 2;

        return true;
    }
}

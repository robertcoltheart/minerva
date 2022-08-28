using System.Buffers.Binary;

namespace Minerva;

public readonly struct ObjectId : IEquatable<ObjectId>
{
    private static readonly byte[] CharToHexLookup;

    private readonly byte[] value;

    static ObjectId()
    {
        CharToHexLookup = CreateCharToHexLookup();
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

    public static ObjectId Parse(ReadOnlySpan<byte> value)
    {
        if (value.Length % 2 != 0)
        {
            throw new ArgumentException();
        }

        var bytes = new byte[20];

        for (var i = 0; i < value.Length / 2; i++)
        {
            var byteLow = CharToHexLookup[i * 2];
            var byteHigh = CharToHexLookup[i * 2 + 1] << 4;

            bytes[i] = (byte)(byteHigh | byteLow);
        }

        return new ObjectId(bytes);
    }

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

        //for (var i = 0; i < count; i++)
        //{
        //    var val = HexLookup[RawId[i + index]];

        //    destination[i * 2] = (char) val;
        //    destination[i * 2 + 1] = (char) (val >> 16);
        //}

        for (var i = 0; i < count; i++)
        {
            var val = RawId[i + index];

            var difference = ((val & 0xf0) << 4) + (val & 0x0f) - 0x8989;
            var packedResult = ((((uint) -difference & 0x7070) >> 4) + difference + 0xb9b9) | 0x2020;

            destination[i * 2] = (char)(packedResult >> 8);
            destination[i * 2 + 1] = (char)(packedResult & 0xff);
        }

        charsWritten = count * 2;

        return true;
    }

    private static byte[] CreateCharToHexLookup()
    {
        var value = new byte[256];

        Array.Fill(value, (byte)0xff);

        for (var i = '0'; i < '9'; i++)
        {
            value[i] = (byte) (i - '0');
        }

        for (var i = 'a'; i < 'f'; i++)
        {
            value[i] = (byte) (i - '0');
        }

        for (var i = 'A'; i < 'F'; i++)
        {
            value[i] = (byte) (i - '0');
        }

        return value;
    }
}

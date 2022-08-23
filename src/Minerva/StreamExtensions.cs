using System.Text;

namespace Minerva;

public static class StreamExtensions
{
    public static ObjectType ReadObjectType(this Stream stream, ObjectId id, Span<byte> buffer)
    {
        var length = stream.ReadToSpaceOrZero(buffer);

        var typeAsString = Encoding.UTF8.GetString(buffer[..length]);

        return typeAsString switch
        {
            "commit " => ObjectType.Commit,
            "tag " => ObjectType.Tag,
            "blob " => ObjectType.Blob,
            "tree " => ObjectType.Tree,
            _ => throw new FormatException($"Invalid git object {id}")
        };
    }

    public static int ReadObjectLength(this Stream stream, ObjectId id, Span<byte> buffer)
    {
        var length = stream.ReadToSpaceOrZero(buffer);

        if (length == 0)
        {
            throw new FormatException($"Invalid git object {id}");
        }

        var size = 0;

        for (var i = 0; i < length; i++)
        {
            size = 10 * size + buffer[i] - '0';
        }

        return size;
    }

    public static int ReadToSpaceOrZero(this Stream stream, Span<byte> buffer)
    {
        var length = 0;

        for (var i = 0; i < buffer.Length; i++)
        {
            var read = stream.Read(buffer.Slice(i, 1));

            if (read != 0 && buffer[i] != 0)
            {
                length++;
            }

            if (buffer[i] == 0 || buffer[i] == ' ' || read == 0)
            {
                break;
            }
        }

        return length;
    }
}

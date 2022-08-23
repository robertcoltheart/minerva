using System.Text;

namespace Minerva;

public static class StreamReaderExtensions
{
    private static bool ReadAndValidate(this StreamReader reader, string value)
    {
        return value.All(x => reader.Read() == x);
    }

    public static ObjectType ReadObjectType(this StreamReader reader, ObjectId id)
    {
        return reader.Peek() switch
        {
            'c' when reader.ReadAndValidate("commit ") => ObjectType.Commit,
            'b' when reader.ReadAndValidate("blob ") => ObjectType.Blob,
            't' when reader.ReadAndValidate("tag ") => ObjectType.Tag,
            _ => throw new InvalidOperationException($"Invalid git object type: {id}")
        };
    }

    public static int ReadObjectLength(this StreamReader reader, ObjectId id)
    {
        var builder = new StringBuilder();

        while (reader.Peek() != 0)
        {
            builder.Append((char) reader.Read());
        }

        reader.Read();

        if (!int.TryParse(builder.ToString(), out var value))
        {
            throw new InvalidOperationException($"Invalid git object length: {id}");
        }

        return value;
    }
}


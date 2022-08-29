namespace Minerva;

public readonly struct ObjectId : IEquatable<ObjectId>
{
    public ObjectId(string value)
    {
        if (value.Length != 40)
        {
            throw new ArgumentException("Invalid SHA value", nameof(value));
        }

        Sha = value;
    }

    public string Sha { get; }

    public bool Equals(ObjectId other)
    {
        return Sha == other.Sha;
    }

    public override bool Equals(object obj)
    {
        return obj is ObjectId objectId && Equals(objectId);
    }

    public override int GetHashCode()
    {
        return Sha.GetHashCode();
    }

    public override string ToString()
    {
        return Sha;
    }

    public string ToPath()
    {
        return $"{Sha[..2]}{Path.DirectorySeparatorChar}{Sha[2..]}";
    }
}

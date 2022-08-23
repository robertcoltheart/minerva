namespace Minerva;

public class Tag : ReferencePointer<GitObject>
{
    public Tag(Reference reference)
        : base(reference)
    {
    }
}

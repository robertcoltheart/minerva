namespace Minerva;

public abstract class ReferencePointer<T>
    where T : GitObject
{
    protected ReferencePointer(Reference reference)
    {
    }
}

namespace Minerva;

public class SymbolicReference : Reference
{
    private readonly Reference reference;

    public SymbolicReference(Reference reference)
    {
        this.reference = reference;
    }

    public override DirectReference? ResolveDirectReference()
    {
        return reference.ResolveDirectReference();
    }
}

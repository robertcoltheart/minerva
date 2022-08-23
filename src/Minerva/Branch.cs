namespace Minerva;

public class Branch : ReferencePointer<Commit>
{
    public Branch(IRepository repository, Reference reference)
        : base(reference)
    {
    }

    public Commit Tip { get; }
}

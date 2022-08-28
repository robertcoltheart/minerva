namespace Minerva;

public class Repository : IRepository
{
    public Repository(string path)
    {
        Info = new RepositoryInformation(path);
        Refs = new ReferenceCollection(this);
        Tags = new TagCollection();
    }

    public Branch Head
    {
        get
        {
            var reference = Refs.Head;

            if (reference == null)
            {
                throw new InvalidOperationException();
            }

            return reference is SymbolicReference
                ? new Branch(this, reference)
                : new DetachedHead(this, reference);
        }
    }

    public ReferenceCollection Refs { get; }

    public TagCollection Tags { get; }

    public RepositoryInformation Info { get; }

    public GitObject? Lookup(ObjectId id)
    {
        throw new NotImplementedException();
    }
}

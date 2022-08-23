namespace Minerva;

public interface IRepository
{
    Branch Head { get; }

    ReferenceCollection Refs { get; }

    TagCollection Tags { get; }

    RepositoryInformation Info { get; }

    GitObject? Lookup(ObjectId id);
}

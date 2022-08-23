namespace Minerva.IO;

public class ObjectDatabase : IObjectDatabase
{
    private readonly IFileSystem[] fileSystems;

    public ObjectDatabase(IRepository repository)
    {
        fileSystems = new IFileSystem[]
        {
            new PackedFileSystem(),
            new LooseFileSystem(repository)
        };
    }

    public RawObject? Read(ObjectId id)
    {
        throw new NotImplementedException();
    }
}

namespace Minerva.IO;

public interface IFileSystem
{
    RawObject? Read(ObjectId id);
}

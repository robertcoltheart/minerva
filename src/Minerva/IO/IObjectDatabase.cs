namespace Minerva.IO;

public interface IObjectDatabase
{
    RawObject? Read(ObjectId id);
}

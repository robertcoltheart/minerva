namespace Minerva;

public static class RepositoryExtensions
{
    public static T? Lookup<T>(this IRepository repository, ObjectId id)
        where T : GitObject
    {
        return repository.Lookup(id) as T;
    }
}

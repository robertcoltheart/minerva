using System.Collections;

namespace Minerva;

public class ReferenceCollection : IEnumerable<Reference>
{
    private static readonly string[] Prefixes =
    {
        string.Empty,
        "refs/",
        "refs/tags/",
        "refs/heads/",
        "refs/remotes/"
    };

    private readonly IRepository repository;

    public ReferenceCollection(IRepository repository)
    {
        this.repository = repository;
    }

    public Reference? this[string name] => Resolve(name);

    public Reference? Head => Resolve("HEAD");

    public IEnumerator<Reference> GetEnumerator()
    {
        var path = Path.Combine(repository.Info.Path, "refs");

        var references = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
            .Select(x => x.Substring(path.Length + 1))
            .Select(Resolve);

        return references.GetEnumerator()!;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private Reference? Resolve(string name)
    {
        foreach (var prefix in Prefixes)
        {
            var refName = $"{prefix}{name}";
            var path = Path.Combine(repository.Info.Path, refName);

            if (File.Exists(path))
            {
                var data = File.ReadAllText(path).TrimEnd('\n');

                if (data.StartsWith("ref: "))
                {
                    var targetName = data[5..];
                    var target = Resolve(targetName);

                    return new SymbolicReference(repository, refName, targetName, target);
                }

                return new DirectReference(refName, repository, new ObjectId(data));
            }
        }

        return null;
    }
}

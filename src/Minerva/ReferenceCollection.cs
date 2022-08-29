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
        return ResolveLoose(name) ?? ResolvePacked(name);
    }

    private Reference? ResolveLoose(string name)
    {
        foreach (var prefix in Prefixes)
        {
            var refName = $"{prefix}{name}";
            var path = Path.Combine(repository.Info.Path, refName);

            if (File.Exists(path))
            {
                var data = File.ReadAllText(path);

                if (data.StartsWith("ref: "))
                {
                    var targetName = data[5..];
                    var target = Resolve(targetName);

                    if (target == null)
                    {
                        return null;
                    }

                    return new SymbolicReference(target);
                }

                var id = new ObjectId(data[..20]);

                return new DirectReference();
            }
        }

        return null;
    }

    private Reference? ResolvePacked(string name)
    {
        var path = Path.Combine(repository.Info.Path, "packed-refs");

        if (File.Exists(path))
        {
            using var reader = new StreamReader(File.OpenRead(path));

            var line = reader.ReadLine();

            while (line != null)
            {
                if (!string.IsNullOrEmpty(line) && line[0] != (byte) '#')
                {
                    var id = line[..40];
                    var refName = line.Substring(id.Length + 1);

                    if (refName == name)
                    {
                        return new DirectReference();
                    }
                }

                line = reader.ReadLine();
            }
        }

        return null;
    }
}

using System.Collections;
using System.Text;

namespace Minerva;

public class ReferenceCollection : IEnumerable<Reference>
{
    private static readonly byte[] RefPrefix = Encoding.UTF8.GetBytes("ref: ");

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
                var data = File.ReadAllBytes(path).AsSpan();

                if (data.StartsWith(RefPrefix))
                {
                    var targetName = Encoding.UTF8.GetString(data.Slice(RefPrefix.Length, data.Length - RefPrefix.Length - 1));
                    var target = Resolve(targetName);

                    if (target == null)
                    {
                        return null;
                    }

                    return new SymbolicReference(target);
                }

                var id = new ObjectId(data[..20].ToArray());

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
            var data = File.ReadAllBytes(path).AsSpan();

            while (data.Length > 0)
            {
                var line = data.SliceLine();

                if (!line.IsEmpty && line[0] != (byte) '#')
                {
                    var id = line[..40];
                    var refName = line.Slice(id.Length + 1);
                    var refNameValue = Encoding.UTF8.GetString(refName);

                    if (refNameValue == name)
                    {
                        return new DirectReference();
                    }
                }

                data = data.Slice(line.Length + 1);
            }
        }

        return null;
    }
}

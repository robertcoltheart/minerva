using System.IO.Compression;

namespace Minerva.IO;

public class LooseFileSystem : IFileSystem
{
    private static readonly byte[] ZlibNoCompression = { 0x78, 0x01 };

    private static readonly byte[] ZlibDefaultCompression = { 0x78, 0x9c };

    private static readonly byte[] ZlibBestCompression = { 0x78, 0xda };

    private readonly IRepository repository;

    private readonly byte[] buffer = new byte[128];

    public LooseFileSystem(IRepository repository)
    {
        this.repository = repository;
    }

    public RawObject? Read(ObjectId id)
    {
        var path = id.ToPath();

        if (!File.Exists(path))
        {
            return null;
        }

        using var stream = File.OpenRead(path);

        var read = stream.Read(buffer.AsSpan(0, 2));

        if (read != 2)
        {
            throw new InvalidOperationException();
        }

        if (buffer[0] != 0x78)
        {
            throw new InvalidOperationException("Invalid zlib header");
        }

        if (buffer[1] != 0x01 && buffer[1] != 0x5e && buffer[1] != 0x9c && buffer[1] != 0xda)
        {
            throw new InvalidOperationException("Invalid zlib header");
        }

        using var deflateStream = new DeflateStream(stream, CompressionMode.Decompress);

        return null;
    }

    private void InitializePath()
    {

    }
}

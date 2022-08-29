using Xunit;

namespace Minerva.Tests;

public class ObjectIdTests
{
    private const string Sha = "7e09f1e930b2a62072d5b23895b098dbf350aa21";

    [Fact]
    public void CanConvertToString()
    {
        var id = new ObjectId(Sha);

        Assert.Equal(Sha, id.ToString());
    }

    [Fact]
    public void CanFormatPath()
    {
        var id = new ObjectId(Sha);

        var path = id.ToPath();

        Assert.Equal($"{Sha[..2]}{Path.DirectorySeparatorChar}{Sha[2..]}", path);
    }
}

using System.Text;
using Xunit;

namespace Minerva.Tests;

public class StreamExtensionsTests
{
    [Fact]
    public void CanReadToFirstSpace()
    {
        const string text = "commit 1234";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var buffer = new byte[256];

        var length = stream.ReadToSpaceOrZero(buffer);

        Assert.Equal(7, length);
        Assert.Equal("commit ", Encoding.UTF8.GetString(buffer, 0, length));
    }

    [Fact]
    public void CanReadToZero()
    {
        const string text = "1234\0897";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var buffer = new byte[256];

        var length = stream.ReadToSpaceOrZero(buffer);

        Assert.Equal(4, length);
        Assert.Equal("1234", Encoding.UTF8.GetString(buffer, 0, length));
    }

    [Fact]
    public void CanReadSpaceAndZero()
    {
        const string text = "commit 1234\0fs";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var typeBuffer = new byte[256];
        var sizeBuffer = new byte[256];

        var typeLength = stream.ReadToSpaceOrZero(typeBuffer);
        var sizeLength = stream.ReadToSpaceOrZero(sizeBuffer);

        Assert.Equal(7, typeLength);
        Assert.Equal("commit ", Encoding.UTF8.GetString(typeBuffer, 0, typeLength));
        Assert.Equal(4, sizeLength);
        Assert.Equal("1234", Encoding.UTF8.GetString(sizeBuffer, 0, sizeLength));
    }

    [Fact]
    public void CanReadToEndOfString()
    {
        const string text = "commit";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var buffer = new byte[256];

        var length = stream.ReadToSpaceOrZero(buffer);

        Assert.Equal(6, length);
        Assert.Equal("commit", Encoding.UTF8.GetString(buffer, 0, length));
    }

    [Fact]
    public void CanReadObjectType()
    {
        const string text = "commit 1234";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var buffer = new byte[256];

        var type = stream.ReadObjectType(default, buffer);

        Assert.Equal(ObjectType.Commit, type);
    }

    [Fact]
    public void InvalidTypeThrows()
    {
        const string text = "invalid 1234";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var buffer = new byte[256];

        var exception = Record.Exception(() => stream.ReadObjectType(default, buffer));

        Assert.NotNull(exception);
    }

    [Fact]
    public void CanReadLength()
    {
        const string text = "1234\0897";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var buffer = new byte[256];

        var length = stream.ReadObjectLength(default, buffer);

        Assert.Equal(1234, length);
    }
}

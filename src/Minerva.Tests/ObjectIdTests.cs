using Xunit;

namespace Minerva.Tests;

public class ObjectIdTests
{
    private const string Sha = "7e09f1e930b2a62072d5b23895b098dbf350aa21";

    [Fact]
    public void CanConvertToString()
    {
        var id = new ObjectId(Convert.FromHexString(Sha));

        Assert.Equal(Sha, id.ToString());
    }

    [Fact]
    public void CanFormatToString()
    {
        var id = new ObjectId(Convert.FromHexString(Sha));

        var result = id.ToString();

        Assert.Equal(Sha, result);
    }

    [Fact]
    public void CanFormatPrefix()
    {
        var id = new ObjectId(Convert.FromHexString(Sha));

        var chars = new char[2];
        var result = id.TryWritePrefix(chars, out var written);

        Assert.True(result);
        Assert.Equal(Sha[..2], new string(chars));
        Assert.Equal(2, written);
    }

    [Fact]
    public void CanFormatId()
    {
        var id = new ObjectId(Convert.FromHexString(Sha));

        var chars = new char[38];
        var result = id.TryWriteId(chars, out var written);

        Assert.True(result);
        Assert.Equal(Sha[2..], new string(chars));
        Assert.Equal(38, written);
    }

    [Fact]
    public void CannotGetIdWithNoCharacters()
    {
        var id = new ObjectId(Convert.FromHexString(Sha));

        var result = id.TryWriteId(Array.Empty<char>(), out var written);

        Assert.False(result);
        Assert.Equal(0, written);
    }

    [Fact]
    public void CannotGetIdWithLessCharacters()
    {
        var id = new ObjectId(Convert.FromHexString(Sha));

        var result = id.TryWriteId(new char[10], out var written);

        Assert.False(result);
        Assert.Equal(0, written);
    }
}

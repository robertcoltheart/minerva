using System.Text;
using Xunit;

namespace Minerva.Tests;

public class SpanExtensionsTests
{
    [Fact]
    public void CanReadSingleLine()
    {
        const string value = "single line";

        var span = Encoding.UTF8.GetBytes(value).AsSpan();

        var line1 = span.SliceLine();
        var line2 = span.Slice(line1.Length + 1);

        Assert.Equal(value.Length, line1.Length);
        Assert.Equal(0, line2.Length);
        Assert.Equal(value, Encoding.UTF8.GetString(line1));
    }

    [Fact]
    public void CanReadMultipleLines()
    {
        const string value = "first\nsecond";

        var span = Encoding.UTF8.GetBytes(value).AsSpan();

        var line1 = span.SliceLine();
        var line2 = span.Slice(line1.Length + 1).SliceLine();

        Assert.Equal("first", Encoding.UTF8.GetString(line1));
        Assert.Equal("second", Encoding.UTF8.GetString(line2));
    }

    [Fact]
    public void CanReadMultipleLinesEndingInNewLine()
    {
        const string value = "first\nsecond\n";

        var span = Encoding.UTF8.GetBytes(value).AsSpan();

        var values = new List<string>();

        while (span.Length > 0)
        {
            var line = span.SliceLine();
            var lineValue = Encoding.UTF8.GetString(line);

            values.Add(lineValue);

            span = span.Slice(line.Length + 1);
        }

        Assert.Equal(2, values.Count);
        Assert.Contains("first", values);
        Assert.Contains("second", values);
    }

    [Fact]
    public void CanReadMultipleLinesEndingInMultipleNewLines()
    {
        const string value = "first\nsecond\n\n\n";

        var span = Encoding.UTF8.GetBytes(value).AsSpan();

        var values = new List<string>();

        while (span.Length > 0)
        {
            var line = span.SliceLine();
            var lineValue = Encoding.UTF8.GetString(line);

            if (!string.IsNullOrEmpty(lineValue))
            {
                values.Add(lineValue);
            }

            span = span.Slice(line.Length + 1);
        }

        Assert.Equal(2, values.Count);
        Assert.Contains("first", values);
        Assert.Contains("second", values);
    }
}

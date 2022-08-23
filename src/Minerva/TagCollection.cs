using System.Collections;

namespace Minerva;

public class TagCollection : IEnumerable<Tag>
{
    public Tag this[string name]
    {
        get { return null; }
    }

    public IEnumerator<Tag> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

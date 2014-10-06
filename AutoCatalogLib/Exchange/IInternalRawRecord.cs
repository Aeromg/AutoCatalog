using System.Collections.Generic;

namespace AutoCatalogLib.Exchange
{
    public interface IInternalRawRecord : IEnumerable<KeyValuePair<string, object>>
    {
        IEnumerable<string> Properties { get; }
        IEnumerable<object> Values { get; }
        object this[string property] { get; }
    }
}
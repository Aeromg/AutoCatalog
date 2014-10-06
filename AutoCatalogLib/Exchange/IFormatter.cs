using System;

namespace AutoCatalogLib.Exchange
{
    public interface IFormatter
    {
        string Name { get; }
        string Description { get; }
        Type Type { get; }
        Guid Guid { get; }
        bool TryFormat(object source, out object destination);
        object Format(object source);
    }

    public interface IFormatter<TReturn> : IFormatter
    {
        bool TryFormat(object source, out TReturn destination);
        new TReturn Format(object source);
    }
}
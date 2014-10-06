namespace AutoCatalogLib.Exchange
{
    public interface IColumn
    {
        int Index { get; }
        string Name { get; }
        bool Active { get; }
        IFormatter Formatter { get; }
        bool Required { get; }
    }
}
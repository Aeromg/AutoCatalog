namespace AutoCatalogLib.Exchange
{
    public interface ISource
    {
        string Location { get; }
        string File { get; }
        void Open();
        void Close();
    }
}

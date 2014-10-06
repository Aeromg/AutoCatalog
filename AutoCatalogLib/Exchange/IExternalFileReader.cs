namespace AutoCatalogLib.Exchange
{
    public interface IExternalFileReader : IExternalReader
    {
        string FileName { get; }
        string FileNameWithPath { get; }
    }
}
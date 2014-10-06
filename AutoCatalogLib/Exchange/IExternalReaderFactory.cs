namespace AutoCatalogLib.Exchange
{
    public interface IExternalReaderFactory
    {
        IExternalReader GetReader(ISource source, IRule behavior);
    }

    public interface IExternalReaderFactory<TReader> : IExternalReaderFactory
        where TReader : IExternalReader
    {
        new TReader GetReader(ISource source, IRule behavior);
    }
}

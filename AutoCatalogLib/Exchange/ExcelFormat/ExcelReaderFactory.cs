using AutoCatalogLib.Exchange.ExcelFormat.Impl;

namespace AutoCatalogLib.Exchange.ExcelFormat
{
    public class ExcelReaderFactory : IExcelReaderFactory
    {
        public ExcelReader GetReader(ISource source, IRule behavior)
        {
            return new ExcelReader(source, (IExcelRule) behavior);
        }

        IExternalReader IExternalReaderFactory.GetReader(ISource source, IRule behavior)
        {
            return GetReader(source, behavior);
        }
    }
}

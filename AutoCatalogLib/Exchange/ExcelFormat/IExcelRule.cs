namespace AutoCatalogLib.Exchange.ExcelFormat
{
    public interface IExcelRule : ITableRule
    {
        int WorksheetIndex { get; }
        bool CsvMode { get; }
    }
}
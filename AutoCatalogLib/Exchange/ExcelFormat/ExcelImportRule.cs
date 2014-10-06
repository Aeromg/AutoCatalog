using System.Collections.Generic;

namespace AutoCatalogLib.Exchange.ExcelFormat
{
    [SourcePointer(ReaderFactory = typeof(IExcelReaderFactory))]
    public class ExcelImportRule : IExcelRule
    {
        public class Column : IColumn
        {
            public string Name { get; set; }
            public int Index { get; set; }
            public IFormatter Formatter { get; set; }
            public bool Active { get; set; }
            public bool Required { get; set; }
        }

        public string Identificator { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ISource Source { get; set; }

        public int RowOffset { get; set; }
        public int ColumnOffset { get; set; }
        public int WorksheetIndex { get; set; }
        public IEnumerable<Column> Columns { get; set; }
        public bool CsvMode { get; set; }

        IEnumerable<IColumn> ITableRule.Columns { get { return Columns; } }
    }
}

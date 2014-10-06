using System.Collections.Generic;
using System.Linq;
using AutoCatalogLib.Modules;

namespace AutoCatalogLib.Exchange
{
    [SingletonModule]
    class RawRecordFormatterFactory : IRawRecordFormatterFactory
    {
        public IRawRecordRowFormatter BuildTableRowFormatter(IEnumerable<IColumn> columns)
        {
            return new RawRecordRowFormatter(columns.Where(c => c.Active).ToDictionary(c => c.Name, c => c.Formatter));
        }


        public IRawRecordRowFormatter BuildTableRowFormatter(ITableRule tableRule)
        {
            return BuildTableRowFormatter(tableRule.Columns);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Modules;

namespace AutoCatalogLib.Exchange
{
    public interface IRawRecordFormatterFactory : IModule
    {
        IRawRecordRowFormatter BuildTableRowFormatter(IEnumerable<IColumn> columns);
        IRawRecordRowFormatter BuildTableRowFormatter(ITableRule tableRule);
    }
}

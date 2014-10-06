using System.Collections.Generic;
using AutoCatalogLib.Exchange;

namespace AutoCatalogLib.Exchange
{
    public interface ITableRule : IRule
    {
        int RowOffset { get; }
        int ColumnOffset { get; }
        IEnumerable<IColumn> Columns { get; }
    }
}

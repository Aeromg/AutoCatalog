using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Business.CatalogSearch
{
    public interface ISearchResult
    {
        IEnumerable<PartItem> Items { get; }
        bool HasMoreItems { get; }
        string Request { get; }
    }
}

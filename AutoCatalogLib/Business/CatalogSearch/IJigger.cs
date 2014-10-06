using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Business.CatalogSearch
{
    public interface IJigger
    {
        ISearchResult Sort(ISearchResult searchResult, Func<PartItem, string> searchPropertyGetter = null);

        IEnumerable<PartItem> Sort(IEnumerable<PartItem> items, string request, Func<PartItem, string> searchPropertyGetter = null);
    }
}

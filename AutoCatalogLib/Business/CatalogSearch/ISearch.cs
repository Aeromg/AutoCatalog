using System.Collections.Generic;
using System.Threading.Tasks;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Business.CatalogSearch
{
    public interface ISearch
    {
        int Limit { get; set; }
        ISearchResult Search(string text);
        ISearchResult SearchAny(IEnumerable<string> text);
        Task<ISearchResult> SearchAsync(string text);
        Task<ISearchResult> SearchAnyAsync(IEnumerable<string> text);
    }
}
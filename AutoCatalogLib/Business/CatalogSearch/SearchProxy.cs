using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Business.CatalogSearch
{
    public class SearchProxy : ISearch
    {
        class ProxySearchResult : ISearchResult
        {
            public static readonly ProxySearchResult Empty = new ProxySearchResult
            {
                HasMoreItems = false,
                Items = new PartItem[0],
                Request = ""
            };

            public IEnumerable<PartItem> Items { get; set; }

            public bool HasMoreItems { get; set; }

            public string Request { get; set; }
        }

        private ISearch _backend;

        public int Limit
        {
            get
            {
                return _backend == null ? 0 : _backend.Limit;
            }
            set 
            { 
                if (_backend == null)
                    return;

                _backend.Limit = value;
            }
        }

        public SearchProxy(ISearch backend = null)
        {
            _backend = backend;
        }

        public ISearchResult Search(string text)
        {
            return _backend != null ? _backend.Search(text) : ProxySearchResult.Empty;
        }

        public ISearchResult SearchAny(IEnumerable<string> text)
        {
            return _backend != null ? _backend.SearchAny(text) : ProxySearchResult.Empty;
        }

        public Task<ISearchResult> SearchAsync(string text)
        {
            return _backend != null ? _backend.SearchAsync(text) : StartEmptyTask();
        }

        public Task<ISearchResult> SearchAnyAsync(IEnumerable<string> text)
        {
            return _backend != null ? _backend.SearchAnyAsync(text) : StartEmptyTask();
        }

        public void SetBackend(ISearch backend)
        {
            _backend = backend;
        }

        private static Task<ISearchResult> StartEmptyTask()
        {
            return Task<ISearchResult>.Factory.StartNew(() => ProxySearchResult.Empty);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AutoCatalogLib.Modules;
using AutoCatalogLib.Modules.Search;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Business.CatalogSearch
{
    public class SimpleJigger : IJigger
    {
        class SortedSearchResult : ISearchResult
        {
            public IEnumerable<PartItem> Items { get; set; }
            public bool HasMoreItems { get; set; }
            public string Request { get; set; }
        }

        private readonly ITextStammer _stammer;
        private readonly Func<PartItem, string> _defaultPropertyGetter;

        public SimpleJigger(ITextStammer stammer = null, Func<PartItem, string> searchPropertyGetter = null)
        {
            _stammer = stammer ?? ModuleLocator.Resolve<ITextStammer>();
            _defaultPropertyGetter = searchPropertyGetter ?? GetDefaultSearchProperty;
        }

        public ISearchResult Sort(ISearchResult searchResult, Func<PartItem, string> searchPropertyGetter = null)
        {
            return new SortedSearchResult
            {
                HasMoreItems = searchResult.HasMoreItems,
                Items = Sort(searchResult.Items, searchResult.Request, searchPropertyGetter),
                Request = searchResult.Request
            };
        }

        public IEnumerable<PartItem> Sort(IEnumerable<PartItem> items, string request, Func<PartItem, string> searchPropertyGetter = null)
        {
            var tokens = GetTokens(request);
            var getProperty = searchPropertyGetter ?? _defaultPropertyGetter;

            return items.OrderByDescending(i => GetWeight(getProperty(i), tokens));
        }

        private string GetDefaultSearchProperty(PartItem partItem)
        {
            return partItem.PartNumber + " " + partItem.SearchString;
        }

        private float GetWeight(string text, string[] tokens)
        {
            var originTokens = GetTokens(text);
            
            float equals = originTokens.Count(tokens.Contains);
            equals = equals/originTokens.Length;

            float starts = (from o in originTokens from t in tokens where o.StartsWith(t) select o).Count();
            starts = starts/originTokens.Length;

            float matches = 
                (from o in originTokens 
                 from t in tokens 
                 where o.Contains(t) 
                 select (float)t.Length / o.Length).Sum();
            matches = matches/_stammer.Stamm(text).Length;

            return (equals * 100 + starts * 5 + matches)/3;
        }

        private string[] GetTokens(string text)
        {
            return text.Split(' ').Select(t => _stammer.Stamm(t)).Where(t => t.Length > 0).ToArray();
        }

    }
}
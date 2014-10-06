using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCatalogLib.Modules.Config;
using AutoCatalogLib.Persist.BusinessModels.Catalog;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Business.CatalogSearch
{
    public class SmartSearch : ISearch, ISmartSearchEngine
    {
        class OrderedSearchResult : ISearchResult
        {
            public IEnumerable<PartItem> Items { get; set; }

            public bool HasMoreItems { get; set; }

            public string Request { get; set; }
        }

        private const int ReadAhead = 1;

        private readonly IJigger _jigger;

        private readonly ISearch _exactPartSearch;

        private readonly ISearch _exactWordSearch;

        private readonly ISearch _fullTextSearch;

        private int _limit;

        public int Limit
        {
            get
            {
                return _limit;
            }
            set
            {
                _limit = value;

                _exactPartSearch.Limit = _limit;
                _exactWordSearch.Limit = _limit;
                _fullTextSearch.Limit = _limit;
            }
        }

        private int ReadLimit { get { return Limit + ReadAhead; } }

        public SmartSearch() : this(new SimpleJigger())
        {
            _exactPartSearch = new ExactPartNumberSearch();
            _exactWordSearch = new ExactSearch();
            _fullTextSearch = new FullTextSearch();
        }

        public SmartSearch(IJigger jigger)
        {
            _jigger = jigger ?? new SimpleJigger();
        }

        public ISearchResult Search(string text)
        {
            if (text == null || text.Trim().Length == 0)
                return new OrderedSearchResult
                {
                    HasMoreItems = false,
                    Items = new PartItem[0],
                    Request = @""
                };

            /* 
             * Точное соответствие по партномеру или по аналогу - вывести в топ.
             * Совпадение начала слова - ранжировать, прикрепить ниже.
             * Соответствие шаблону - ранжировать, отдать в хвост.
             */

            var exactPart = _exactPartSearch.Search(text).Items;
            var exactWord = _jigger.Sort(_exactWordSearch.Search(text).Items, text);
            
            var fullTextSearchResult = _fullTextSearch.Search(text);
            var fullText = _jigger.Sort(fullTextSearchResult.Items, text);

            return new OrderedSearchResult
            {
                HasMoreItems = fullTextSearchResult.HasMoreItems,
                Items = JoinAndDistinct(new[] {exactPart, exactWord, fullText}),
                Request = text
            };
        }

        private IEnumerable<PartItem> JoinAndDistinct(IEnumerable<IEnumerable<PartItem>> collections)
        {
            var partNums = new HashSet<string>();
            var distincted = new List<PartItem>(_limit);

            foreach (var item in collections.SelectMany(i => i.Select(p => p)))
            {
                if(partNums.Contains(item.PartNumber))
                    continue;

                partNums.Add(item.PartNumber);
                distincted.Add(item);
            }

            return distincted.AsEnumerable();
        }

        public Task<ISearchResult> SearchAsync(string text)
        {
            return Task.Factory.StartNew(() => Search(text));
        }

        public Task<ISearchResult> SearchAnyAsync(IEnumerable<string> text)
        {
            return Task<ISearchResult>.Factory.StartNew(() => SearchAny(text));
        }

        public ISearchResult SearchAny(IEnumerable<string> text)
        {
            var items = text.SelectMany(t => Search(t).Items).Distinct(new EntityEqualityComparer<PartItem>())
                .Take(ReadLimit).ToArray();

            return new OrderedSearchResult
            {
                HasMoreItems = items.Length > Limit,
                Items = items.Take(Limit),
                Request = @"<multiple>"
            };
        }
    }
}
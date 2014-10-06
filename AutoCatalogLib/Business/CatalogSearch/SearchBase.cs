using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Modules;
using AutoCatalogLib.Modules.Search;
using AutoCatalogLib.Persist;
using AutoCatalogLib.Persist.BusinessModels.Catalog;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Business.CatalogSearch
{
    public abstract class SearchBase : ISearch
    {
        class SearchResult : ISearchResult
        {
            public IEnumerable<PartItem> Items { get; set; }

            public bool HasMoreItems { get; set; }

            public string Request { get; set; }
        }

        private const int ReadAhead = 1;

        private static Context _context;

        protected static readonly ISqlWhereClauseFormatter SqlFormatter =
            ModuleLocator.Resolve<ISqlWhereClauseFormatter>();

        private ISearchResult _reduceableSearchResult = null;

        private readonly object _lockObject = new object();

        private int _limit;

        public int Limit
        {
            get { return _limit; }
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException();

                _limit = value;
            }
        }

        private int ReadLimit { get { return Limit + ReadAhead; } }

        protected abstract IEnumerable<string> GetSearchTokens(string text);

        /* 
         * ToDo: GetSqlWhereClause и FilterItemsByToken
         * Странно пахнущая пара методов из-за проблем в ADO.NET-реализации метода LIKE у SqLite.
         * Linq вместо нормального LIKE форматируется в специфичные для T-SQL фразы, которые ведут себя некорректно.
         * За ненадобностью SqLite вся низкоуровневая фигня должна быть выпилена нафиг.
         */
        protected abstract string GetSqlWhereClause(IEnumerable<string> tokens);

        protected abstract IQueryable<PartItem> FilterItemsByToken(IQueryable<PartItem> items, string token);

        public Task<ISearchResult> SearchAsync(string text)
        {
            return Task<ISearchResult>.Factory.StartNew(() => Search(text));
        }

        public ISearchResult Search(string text)
        {
            lock (_lockObject)
            {
                if(text == null || text.Trim().Length == 0)
                    return new SearchResult
                    {
                        HasMoreItems = false,
                        Items = new PartItem[0],
                        Request = @""
                    };

                var searchResult = IsReducableRequest(text)
                    ? ReduceableSearch(text)
                    : FullscopeSearch(text);

                _reduceableSearchResult = !searchResult.HasMoreItems ? searchResult : null;

                return searchResult;
            }
        }

        private ISearchResult ReduceableSearch(string text)
        {
            var tokens = GetSearchTokens(text);

            var items = FilterItems(_reduceableSearchResult.Items.AsQueryable(), text);
            return new SearchResult
            {
                HasMoreItems = false,
                Items = items,
                Request = text
            };
        }

        private ISearchResult FullscopeSearch(string text)
        {
            var tokens = GetSearchTokens(text);

            var items = GetContext().PartItems.SqlAppend(GetSqlWhereClause(tokens)).Take(ReadLimit);

            return new SearchResult
            {
                HasMoreItems = items.Count() > Limit,
                Items = items.Take(Limit),
                Request = text
            };
        }

        private IQueryable<PartItem> FilterItems(IQueryable<PartItem> itemsSource, string text)
        {
            IQueryable<PartItem> items = itemsSource;

            foreach (var token in GetSearchTokens(text))
                items = FilterItemsByToken(items, token);

            return items;
        }

        private bool IsReducableRequest(string text)
        {
            if (_reduceableSearchResult == null)
                return false;

            return !_reduceableSearchResult.HasMoreItems && text.StartsWith(_reduceableSearchResult.Request);
        }

        public Task<ISearchResult> SearchAnyAsync(IEnumerable<string> text)
        {
            return Task<ISearchResult>.Factory.StartNew(() => SearchAny(text));
        }

        public ISearchResult SearchAny(IEnumerable<string> text)
        {
            var items = text.SelectMany(t => Search(t).Items).Distinct(new EntityEqualityComparer<PartItem>())
                .Take(ReadLimit).ToArray();

            return new SearchResult
            {
                HasMoreItems = items.Length > Limit,
                Items = items.Take(Limit),
                Request = @"<multiple>"
            };
        }

        private static Context GetContext()
        {
            if(_context == null)
                _context = new Context();

            if (_context.IsOutdated)
            {
                _context.Dispose();
                _context = new Context();
            }

            return _context;
        }
    }
}

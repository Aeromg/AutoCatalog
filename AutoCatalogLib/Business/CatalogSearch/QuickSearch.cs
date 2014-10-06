using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCatalogLib.Modules;
using AutoCatalogLib.Modules.Config;
using AutoCatalogLib.Modules.Search;
using AutoCatalogLib.Persist;
using AutoCatalogLib.Persist.BusinessModels.Catalog;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Business.CatalogSearch
{
    class QuickSearch : ISearch, IQuickSearchEngine
    {
        class QuickSearchResult : ISearchResult
        {
            public static readonly QuickSearchResult Empty = new QuickSearchResult
            {
                HasMoreItems = false,
                Items = new PartItem[0],
                Request = ""
            };

            public IEnumerable<PartItem> Items { get; set; }

            public bool HasMoreItems { get; set; }

            public string Request { get; set; }

            public bool NoReduce { get; set; }
        }

        private static Context _context;

        private const int ReadAhead = 1;

        private static readonly ISqlWhereClauseFormatter SqlFormatter =
            ModuleLocator.Resolve<ISqlWhereClauseFormatter>();

        private static readonly ITextStammer Stammer = ModuleLocator.Resolve<ITextStammer>();

        private ISearchResult _reduceableSearchResult = null;

        private readonly object _lockObject = new object();

        private int _limit;

        public int Limit
        {
            get { return _limit; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();

                _limit = value;
            }
        }

        private int ReadLimit { get { return Limit + ReadAhead; } }

        public ISearchResult Search(string text)
        {
            lock (_lockObject)
            {
                if (text == null || text.Trim().Length == 0)
                    return QuickSearchResult.Empty;

                var searchResult = IsReducableRequest(text) ? ReduceableSearch(text) : FullscopeSearch(text);

                if (!searchResult.NoReduce)
                    _reduceableSearchResult = !searchResult.HasMoreItems ? searchResult : null;

                return searchResult;
            }
        }

        private QuickSearchResult FullscopeSearch(string text)
        {
            var tokens = GetSearchTokens(text);
            var items = tokens.Length == 1 ? SearchSingle(tokens[0]) : SearchAll(tokens);

            return new QuickSearchResult
            {
                HasMoreItems = items.Count() > Limit,
                Items = items.Take(Limit).ToArray(),
                Request = text,
                NoReduce = tokens.Length == 1
            };
        }

        private QuickSearchResult ReduceableSearch(string text)
        {
            var tokens = GetSearchTokens(text);
            var items = tokens.Length == 1 
                ? ReduceSingle(_reduceableSearchResult.Items, tokens[0]) 
                : ReduceAll(_reduceableSearchResult.Items, tokens);

            return new QuickSearchResult
            {
                HasMoreItems = false,
                Items = items,
                Request = text,
                NoReduce = tokens.Length == 1
            };
        }

        private IQueryable<PartItem> SearchSingle(string token)
        {
            var where = SqlFormatter.Disjunction(
                SqlFormatter.FormatStartsWith(@"StammedPartNumber", token),
                SqlFormatter.FormatStartsWith(@"Analogs", token),
                SqlFormatter.FormatContains(@"Analogs", " " + token));

            return GetContext().PartItems.SqlAppend(where).Take(ReadLimit);
        }

        private IQueryable<PartItem> SearchAll(IEnumerable<string> tokens)
        {
            var where = SqlFormatter.Conjunction(tokens.Select(t => SqlFormatter.FormatContains(@"SearchString", t)));
            return GetContext().PartItems.SqlAppend(where).Take(ReadLimit);
        }

        private IEnumerable<PartItem> ReduceSingle(IEnumerable<PartItem> items, string token)
        {
            return
                from item in items
                where
                    item.StammedPartNumber.StartsWith(token) ||
                    item.Analogs.StartsWith(token) ||
                    item.Analogs.Contains(" " + token)

                select item;
        }

        private IEnumerable<PartItem> ReduceAll(IEnumerable<PartItem> items, IEnumerable<string> tokens)
        {
            IEnumerable<PartItem> reduced = items;

            foreach (var token in tokens)
                reduced = ReduceAllIteration(reduced, token);

            return reduced;
        }

        private IEnumerable<PartItem> ReduceAllIteration(IEnumerable<PartItem> items, string token)
        {
            return items.Where(i => i.SearchString.Contains(token));
        }

        private QuickSearchResult SearchMultiple(string token, string request)
        {
            return QuickSearchResult.Empty;
        }

        public Task<ISearchResult> SearchAsync(string text)
        {
            return Task.Factory.StartNew(() => Search(text));
        }

        private string[] GetSearchTokens(string text)
        {
            return text.Split(' ').Select(t => Stammer.Stamm(t)).Where(t => t.Length > 0).ToArray();
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

            return new QuickSearchResult
            {
                HasMoreItems = items.Length > Limit,
                Items = items.Take(Limit),
                Request = @"<multiple>"
            };
        }

        private static Context GetContext()
        {
            if (_context == null)
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
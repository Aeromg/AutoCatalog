using System;
using System.Collections.Generic;
using System.Linq;
using AutoCatalogLib.Modules;
using AutoCatalogLib.Modules.Config;
using AutoCatalogLib.Modules.Search;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Business.CatalogSearch
{
    public class ExactPartNumberSearch : SearchBase, IAnalogSearchEngine
    {
        private static readonly ITextStammer Stammer = ModuleLocator.Resolve<ITextStammer>();

        protected override IEnumerable<string> GetSearchTokens(string text)
        {
            return text.Split(' ').Select(t => Stammer.Stamm(t)).Where(t => t.Length > 0).ToArray();
        }

        protected override string GetSqlWhereClause(IEnumerable<string> tokens)
        {
            return SqlFormatter.Conjunction(tokens.Select(t => SqlFormatter.FormatEquals("StammedPartNumber", t)));
        }

        protected override IQueryable<PartItem> FilterItemsByToken(IQueryable<PartItem> items, string token)
        {
            return
                from item in items
                where item.StammedPartNumber == token
                select item;
        }
    }
}
using System;
using System.Linq;
using AutoCatalogLib.Business.CatalogImport;
using AutoCatalogLib.Modules;
using AutoCatalogLib.Modules.Search;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Exchange
{
    [SingletonModule]
    class PartItemFactory : IPartItemFactory
    {
        private static readonly ITextStammer Stammer 
            = ModuleLocator.Resolve<ITextStammer>();

        private static readonly IPartItemIndexFormatter IndexFormatter
            = ModuleLocator.Resolve<IPartItemIndexFormatter>();

        public PartItem BuildFrom(CatalogImportItem item)
        {
            var analogs = String.Join(" ", item.Analogs.Select(a => a.Replace(" ", "")));

            var partItem = new PartItem()
            {
                Balance = item.Balance,
                Brand = item.Brand,
                Commentary = item.Commentary,
                Name = item.Name,
                Price = item.Price,
                PartNumber = item.PartNumber,
                StammedPartNumber = Stammer.Stamm(item.PartNumber),
                SourceArgument = item.SourceArgument,
                Analogs = analogs
            };

            partItem.SearchString = IndexFormatter.GetIndex(partItem);

            return partItem;
        }
    }
}
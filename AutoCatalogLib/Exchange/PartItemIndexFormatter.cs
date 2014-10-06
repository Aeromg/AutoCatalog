using System;
using System.Linq;
using AutoCatalogLib.Modules;
using AutoCatalogLib.Modules.Search;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Exchange
{
    class PartItemIndexFormatter : IPartItemIndexFormatter
    {
        private readonly ITextStammer Stammer = ModuleLocator.Resolve<ITextStammer>();

        public string GetIndex(PartItem partItem)
        {
            return Stammer.Stamm(partItem.PartNumber)
                + @" " + String.Join(" ", partItem.Name.Split(' ').Select(Stammer.Stamm))
                + @" " + Stammer.Stamm(partItem.Brand)
                + @" " + String.Join(@" ", partItem.Analogs.Split(' ').Select(Stammer.Stamm));
        }
    }
}
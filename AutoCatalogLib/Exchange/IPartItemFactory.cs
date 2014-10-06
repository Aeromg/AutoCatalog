using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Business.CatalogImport;
using AutoCatalogLib.Modules;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Exchange
{
    public interface IPartItemFactory : IModule
    {
        PartItem BuildFrom(CatalogImportItem item);
    }
}

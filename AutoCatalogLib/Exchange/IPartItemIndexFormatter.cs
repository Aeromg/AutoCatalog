using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Modules;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Exchange
{
    public interface IPartItemIndexFormatter : IModule
    {
        string GetIndex(PartItem partItem);
    }
}

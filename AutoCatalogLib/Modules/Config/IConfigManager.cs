using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Business.CatalogSearch;

namespace AutoCatalogLib.Modules.Config
{
    public interface IConfigManager : IModule
    {
        ISearch GetPrimarySearchEngine();
        ISearch GetAnalogsSearchEngine();
        
        int SearchResultsLimit { get; set; }
        bool UseSmartPrimarySearchEngine { get; set; }
        bool UseRecursiveAnalogsSearchEngine { get; set; }
        bool EnableViewItemSource { get; set; }
        bool EnableGuiImport { get; set; }
        string ProtectConfigPassword { get; set; }

        void LoadConfig();
        void SaveConfig();
    }
}

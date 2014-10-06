using System.Linq.Expressions;
using System.Windows.Input;
using AutoCatalogLib;
using AutoCatalogLib.Modules.Config;

namespace AutoCatalogWpf.ViewModels.SearchConfig
{
    public class SearchConfigViewModel : ViewModel
    {
        private readonly IConfigManager _config;

        public bool UseSmartPrimarySearchEngine { get; set; }
        public bool UseRecursiveAnalogsSearchEngine { get; set; }
        public int SearchResultsLimit { get; set; }
        public bool EnableViewItemSource { get; set; }
        public bool EnableGuiImport { get; set; }
        public string ProtectConfigPassword { get; set; }

        private Command _saveCommand;
        public Command Save
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new Command
                {
                    ExecuteAction = SaveImpl,
                    CanExecuteFunc = IsChanged
                });
            }
        }

        public SearchConfigViewModel()
            : this(Config.Manager)
        {
            PropertyChanged += (sender, args) => Save.OnCanExecuteChanged();
        }

        public SearchConfigViewModel(IConfigManager config)
        {
            _config = config;
            Restore();
        }

        private void Restore()
        {
            UseSmartPrimarySearchEngine = _config.UseSmartPrimarySearchEngine;
            UseRecursiveAnalogsSearchEngine = _config.UseRecursiveAnalogsSearchEngine;
            SearchResultsLimit = _config.SearchResultsLimit;
            EnableViewItemSource = _config.EnableViewItemSource;
            EnableGuiImport = _config.EnableGuiImport;
            ProtectConfigPassword = _config.ProtectConfigPassword;
        }

        private void SaveImpl()
        {
            _config.UseSmartPrimarySearchEngine = UseSmartPrimarySearchEngine;
            _config.UseRecursiveAnalogsSearchEngine = UseRecursiveAnalogsSearchEngine;
            _config.SearchResultsLimit = SearchResultsLimit;
            _config.EnableViewItemSource = EnableViewItemSource;
            _config.EnableGuiImport = EnableGuiImport;
            _config.ProtectConfigPassword = ProtectConfigPassword;   // да, так просто.
            
            _config.SaveConfig();

            Save.OnCanExecuteChanged();
        }

        private bool IsChanged()
        {
            return 
                UseSmartPrimarySearchEngine != _config.UseSmartPrimarySearchEngine ||
                UseRecursiveAnalogsSearchEngine != _config.UseRecursiveAnalogsSearchEngine ||
                SearchResultsLimit != _config.SearchResultsLimit ||
                EnableViewItemSource != _config.EnableViewItemSource ||
                EnableGuiImport != _config.EnableGuiImport ||
                ProtectConfigPassword != _config.ProtectConfigPassword;
        }
    }
}

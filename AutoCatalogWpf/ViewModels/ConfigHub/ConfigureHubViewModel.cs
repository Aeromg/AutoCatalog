using System.Reflection;
using AutoCatalogWpf.ViewModels.Converters;
using AutoCatalogWpf.ViewModels.Rules;
using AutoCatalogWpf.ViewModels.SearchConfig;
using AutoCatalogWpf.ViewModels.SourcesProfiles;

namespace AutoCatalogWpf.ViewModels.ConfigHub
{
    public class ConfigureHubViewModel : ViewModel
    {
        private Command _closePage;

        public Command ClosePage
        {
            get
            {
                return _closePage ?? (_closePage = new Command
                {
                    ExecuteAction = () => App.Window.GoBack()
                });
            }
        }

        public ConvertersSummaryViewModel Converters { get; set; }

        public RulesSummaryViewModel Rules { get; set; }

        public ImportTargetsSummaryViewModel Targets { get; set; }

        public SearchConfigViewModel Search { get; set; }

        public ConfigureHubViewModel()
        {
            Reset();
            App.Window.ModalClosed += (sender, args) => Reset();
        }

        private void Reset()
        {
            Converters = new ConvertersSummaryViewModel();
            Rules = new RulesSummaryViewModel();
            Targets = new ImportTargetsSummaryViewModel();
            Search = new SearchConfigViewModel();
        }
    }
}
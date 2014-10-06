using System.Windows.Controls;
using AutoCatalogWpf.ViewModels.SourcesProfiles;

namespace AutoCatalogWpf.Views.SourcesProfiles
{
    /// <summary>
    /// Interaction logic for FileTargetPage.xaml
    /// </summary>
    public partial class FileTargetPage : Page
    {
        public FileTargetPage() : this(new FileImportTargetViewModel()) { }

        public FileTargetPage(FileImportTargetViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

using System.Windows.Controls;
using AutoCatalogWpf.ViewModels.Converters;

namespace AutoCatalogWpf.Views.Converters
{
    /// <summary>
    ///     Interaction logic for ConverterEditPage.xaml
    /// </summary>
    public partial class ConverterEditPage : Page
    {
        public ConverterEditPage() : this(new ConverterViewModel())
        {
        }

        public ConverterEditPage(ConverterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
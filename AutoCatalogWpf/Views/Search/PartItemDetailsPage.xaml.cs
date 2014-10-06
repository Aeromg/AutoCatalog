using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoCatalogWpf.ViewModels.Search;

namespace AutoCatalogWpf.Views.Search
{
    /// <summary>
    ///     Interaction logic for PartItemDetailsPage.xaml
    /// </summary>
    public partial class PartItemDetailsPage : Page
    {
        private readonly PartItemViewModel _viewModel;

        public PartItemDetailsPage(PartItemViewModel viewModel) : this()
        {
            _viewModel = viewModel;
            DataContext = _viewModel;
            _viewModel.FetchDetails();
        }

        public PartItemDetailsPage()
        {
            InitializeComponent();
        }

        private void VirtualAnalogsList_OnSelected(object sender, RoutedEventArgs e)
        {
            if (VirtualAnalogsList.SelectedIndex != -1)
                RealAnalogsList.SelectedIndex = -1;
        }

        private void RealAnalogsList_OnSelected(object sender, RoutedEventArgs e)
        {
            if (RealAnalogsList.SelectedIndex != -1)
                VirtualAnalogsList.SelectedIndex = -1;
        }

        private void RealAnalogsList_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.OnItemAction();
        }
    }
}
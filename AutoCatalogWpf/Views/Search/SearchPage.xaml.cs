using System.Windows.Controls;
using System.Windows.Input;
using AutoCatalogWpf.ViewModels.Search;

namespace AutoCatalogWpf.Views.Search
{
    /// <summary>
    ///     Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        private readonly SearchViewModel _model;

        public SearchPage() : this(new SearchViewModel())
        {
        }

        public SearchPage(SearchViewModel model)
        {
            InitializeComponent();
            _model = model;
            DataContext = model;
            App.Window.ModalClosed += (sender, args) => SearchBox.Focus();
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_model != null)
                _model.OnItemAction();
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!e.IsDown || _model == null)
                return;

            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                    break;
                case Key.Enter:
                    _model.OnItemAction();
                    break;
                default:
                    _model.OnKey(e.Key);
                    break;
            }
        }

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!e.IsDown || _model == null)
                return;

            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                    _model.OnKey(e.Key);
                    break;
            }
        }
    }
}
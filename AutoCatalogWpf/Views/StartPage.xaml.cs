using AutoCatalogWpf.ViewModels;

namespace AutoCatalogWpf.Views
{
    /// <summary>
    ///     Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage
    {
        public StartPage() : this(new StartViewModel())
        {
        }

        public StartPage(StartViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
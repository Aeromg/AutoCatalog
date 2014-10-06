using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoCatalogWpf.ViewModels.Import;

namespace AutoCatalogWpf.Views.Import
{
    /// <summary>
    /// Interaction logic for ImportProgressPage.xaml
    /// </summary>
    public partial class ImportProgressPage : Page
    {
        public ImportProgressPage()
        {
            InitializeComponent();
        }

        public ImportProgressPage(ImportProgressViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}

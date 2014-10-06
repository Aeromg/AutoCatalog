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

namespace AutoCatalogWpf.Views.Import
{
    /// <summary>
    /// Interaction logic for ImportHubPage.xaml
    /// </summary>
    public partial class ImportHubPage : Page
    {
        public ImportHubPage()
        {
            InitializeComponent();

            PresetsListBox.SelectionChanged += (sender, args) =>
            {
                if (PresetsListBox.SelectedIndex != -1)
                    CustomsListBox.SelectedIndex = -1;
            };

            CustomsListBox.SelectionChanged += (sender, args) =>
            {
                if (CustomsListBox.SelectedIndex != -1)
                    PresetsListBox.SelectedIndex = -1;
            };
        }
    }
}

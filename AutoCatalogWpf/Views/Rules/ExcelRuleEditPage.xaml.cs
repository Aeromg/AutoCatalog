using System.Windows.Controls;
using AutoCatalogWpf.ViewModels.Rules;

namespace AutoCatalogWpf.Views.Rules
{
    /// <summary>
    ///     Interaction logic for RuleEditPage.xaml
    /// </summary>
    public partial class ExcelRuleEditPage : Page
    {
        public ExcelRuleEditPage() : this(new ExcelRuleViewModel())
        {
        }

        public ExcelRuleEditPage(ExcelRuleViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
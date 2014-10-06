using System;
using System.Windows.Controls;

namespace AutoCatalogWpf.Views
{
    /// <summary>
    ///     Interaction logic for ErrorPage.xaml
    /// </summary>
    public partial class ErrorPage : Page
    {
        public ErrorPage(Exception ex)
        {
            InitializeComponent();

            ExceptionField.Text = String.Format(@"{0}: {1}", ex.GetType(), ex.Message);
            DetailsField.Text = String.Format(ex.ToString());
        }
    }
}
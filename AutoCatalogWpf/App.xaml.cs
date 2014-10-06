using System.Windows;
using AutoCatalogLib;
using AutoCatalogWpf.Views;

namespace AutoCatalogWpf
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            TestUtils.SimpleStupidLocker();
            /*
            DispatcherUnhandledException += (sender, args) =>
            {
                try
                {
                    Window.NavigateModal(new ErrorPage(args.Exception));
                    args.Handled = true;
                }
                catch
                {
                }
            };*/ 

            //AutoCatalogLib.TestUtils.DefaultRules();
        }

        public new static App Current
        {
            get { return (App) Application.Current; }
        }

        public static MainWindow Window
        {
            get { return (MainWindow) Current.MainWindow; }
        }
    }
}
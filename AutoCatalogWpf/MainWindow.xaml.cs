using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoCatalogLib.Exchange.ImportTargets;
using AutoCatalogWpf.Utils;
using AutoCatalogWpf.Views;
using AutoCatalogWpf.Views.ConfigHub;
using AutoCatalogWpf.Views.Import;
using AutoCatalogWpf.Views.Search;

namespace AutoCatalogWpf
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [Magic]
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Page _modalPage;
        private Page _currentPage;

        public MainWindow()
        {
            InitializeComponent();
            ReleaseInfoLabel.Content = VersionInfo.GetVersionByLinkerTimestamp();
            Navigate(new SearchPage());
            //Navigate(new ConfigureHubPage());
            IsProgress = false;
        }

        #region Main frame

        private readonly Stack<Page> _navigationHistory = new Stack<Page>();

        public void Navigate(Page page)
        {
            _navigationHistory.Push(_currentPage);
            _currentPage = page;
            Frame.Navigate(page);
            NavigateModal(null);
        }

        public void GoBack()
        {
            _currentPage = _navigationHistory.Pop();
            Frame.Navigate(_currentPage);
            NavigateModal(null);
        }

        #endregion

        #region Modal frame

        public void NavigateModal(Page page)
        {
            if (page == null)
            {
                ModalFrame.Visibility = Visibility.Collapsed;
                Frame.IsEnabled = true;
                OnModalClosed();
            }
            else
            {
                ModalFrame.Visibility = Visibility.Visible;
                Frame.IsEnabled = false;
                OnModalOpened();
            }
            ModalFrame.Navigate(page);
            _modalPage = page;
            ModalGrid.Visibility = ModalFrame.Visibility;
        }

        public event EventHandler ModalClosed;
        public event EventHandler ModalOpened;

        private void OnModalOpened()
        {
            var handler = ModalOpened;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnModalClosed()
        {
            var handler = ModalClosed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        public bool IsProgress
        {
            get { return ProgressBar.Visibility == Visibility.Visible; }
            set
            {
                ProgressBar.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                RaisePropertyChanged("IsProgress");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_modalPage != null && e.Key == Key.Escape)
                NavigateModal(null);
        }

        public virtual void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(property));
        }
    }
}
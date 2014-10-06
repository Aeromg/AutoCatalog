using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoCatalogLib;
using AutoCatalogLib.Business;
using AutoCatalogLib.Business.CatalogSearch;
using AutoCatalogLib.Persist.BusinessModels.Catalog;
using AutoCatalogWpf.Views;
using AutoCatalogWpf.Views.ConfigHub;
using AutoCatalogWpf.Views.Import;
using AutoCatalogWpf.Views.Search;

namespace AutoCatalogWpf.ViewModels.Search
{
    public class SearchViewModel : ViewModel
    {
        private readonly ISearch _search = Config.Manager.GetPrimarySearchEngine();

        private bool _busy;
        private bool Busy
        {
            get { return _busy; }
            set
            {
                _busy = value;
                App.Window.IsProgress = _busy;
            }
        }

        private string _searchStringExecuted;
        private Task _searchTask;
        public PartItemViewModel _selectedItem;

        public SearchViewModel()
            : this(String.Empty)
        {
        }

        public SearchViewModel(string searchString)
        {
            Items = new ObservableCollection<PartItemViewModel>();
            SelectedItem = null;

            SubscribeEvents();
            BuildCommands();
            SearchString = searchString;
        }

        public string SearchString { get; set; }
        public Command Search { get; private set; }
        public Command ClosePage { get; private set; }

        public Command GoConfigHub
        {
            get
            {
                return new Command
                {
                    ExecuteAction = () => App.Window.Navigate(new ConfigureHubPage())
                };
            }
        }
        public Command GoUpdate
        {
            get
            {
                return new Command
                {
                    ExecuteAction = () => App.Window.Navigate(new ImportHubPage())
                };
            }
        }

        public ObservableCollection<PartItemViewModel> Items { get; private set; }

        public PartItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != null)
                    _selectedItem.IsSelected = false;

                _selectedItem = value;
                if (_selectedItem != null)
                    _selectedItem.IsSelected = true;
            }
        }

        public bool TooMuchResults { get; set; }
        public bool NoResults { get; set; }

        private void BuildCommands()
        {
            Search = new Command
            {
                CanExecuteFunc = () => SearchString != null && SearchString.Trim().Length > 0,
                ExecuteAction = SearchImpl
            };

            ClosePage = new Command
            {
                ExecuteAction = ClosePageImpl
            };
        }

        private void SubscribeEvents()
        {
            PropertyChanged += (sender, arg) =>
            {
                if (sender != this)
                    return;

                switch (arg.PropertyName)
                {
                    case @"SearchString":
                        SearchImpl();
                        Search.OnCanExecuteChanged();
                        break;
                    case @"SelectedItem":
                        break;
                }
            };
        }

        private async void SearchImpl()
        {
            if (Busy)
                return;

            Busy = true;

            ISearchResult searchResult = null;

            while (searchResult == null || searchResult.Request != SearchString)
            {
                searchResult = await _search.SearchAsync(SearchString);
                UpdateOutput(searchResult);
            }

            Busy = false;
        }

        private void ClosePageImpl()
        {
            App.Window.GoBack();
        }

        private void UpdateOutput(ISearchResult searchResult)
        {
            Items.Clear();
            foreach (var backedItem in searchResult.Items)
            {
                Items.Add(new PartItemViewModel(backedItem));
            }

            TooMuchResults = searchResult.HasMoreItems;
            NoResults = Items.Count == 0;

            //var toRemove = Items.Where(i => !backedItems.Contains(i.PartItem)).ToArray();

            /*foreach (var removedItem in toRemove)
                Items.Remove(removedItem);

            foreach (var backedItem in backedItems)
                if (Items.All(i => i.PartItem != backedItem))
                    Items.Add(new PartItemViewModel(backedItem));

            if (SelectedItem != null && !Items.Contains(SelectedItem))
                SelectedItem = null;

            if (Items.Count == 1)
                SelectedItem = Items.First(); */
        }

        public void OnItemAction()
        {
            if (SelectedItem != null)
                ShowDetails(SelectedItem);
        }

        private void ShowDetails(PartItemViewModel item)
        {
            App.Window.NavigateModal(new PartItemDetailsPage(item));
        }

        public void OnKey(Key key)
        {
            switch (key)
            {
                case Key.Up:
                    SelectedUp();
                    break;

                case Key.Down:
                    SelectedDown();
                    break;
            }
        }

        private void SelectedUp()
        {
            if (Items.Count == 0)
                return;

            var index = SelectedItem == null ? -1 : Items.IndexOf(SelectedItem) - 1;
            if (index < 0)
                index = Items.Count - 1;

            SelectedItem = Items[index];
        }

        private void SelectedDown()
        {
            if (Items.Count == 0)
                return;

            var index = SelectedItem == null ? 0 : Items.IndexOf(SelectedItem) + 1;
            if (index >= Items.Count)
                index = 0;

            SelectedItem = Items[index];
        }
    }
}
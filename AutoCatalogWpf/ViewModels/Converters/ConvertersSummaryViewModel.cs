using System;
using System.Collections.ObjectModel;
using System.Linq;
using AutoCatalogLib.Business;
using AutoCatalogWpf.Views;
using AutoCatalogWpf.Views.Converters;

namespace AutoCatalogWpf.ViewModels.Converters
{
    public class ConvertersSummaryViewModel : ViewModel
    {
        private Command _deleteCommand;
        private Command _editCommand;
        private ConverterViewModel _selectedItem;

        public ConvertersSummaryViewModel()
        {
            Converters = new ObservableCollection<ConverterViewModel>();
                //FormattersLocator.GetFormatters().Select(ConverterViewModel.FromModel));
            UpdateConverters();
        }

        #region CRUD methods

        private void CreateImpl()
        {
            App.Window.ModalClosed += OnEditorClosed;
            App.Window.NavigateModal(new ConverterEditPage());
        }

        private void OnEditorClosed(object sender, EventArgs e)
        {
            App.Window.ModalClosed -= OnEditorClosed;
            UpdateConverters();
        }

        private void UpdateConverters()
        {
            var converters = FormattersLocator.GetFormatters().Select(ConverterViewModel.FromModel);
            Converters = new ObservableCollection<ConverterViewModel>(
                converters.OrderBy(c => c.IsFixed).ThenBy(c => c.Name));
        }

        private void EditImpl()
        {
            App.Window.ModalClosed += OnEditorClosed;
            App.Window.NavigateModal(new ConverterEditPage(SelectedItem));
        }

        private void DeleteImpl()
        {
            var formatter = FormattersLocator.Get(SelectedItem.Guid);
            FormattersLocator.Remove(formatter);
            UpdateConverters();
        }

        #endregion

        public ObservableCollection<ConverterViewModel> Converters { get; private set; }

        public ConverterViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value)
                    return;

                _selectedItem = value;

                RaisePropertyChanged("CanModify");
                Edit.OnCanExecuteChanged();
                Delete.OnCanExecuteChanged();
            }
        }

        public Command Create
        {
            get
            {
                return new Command
                {
                    ExecuteAction = CreateImpl
                };
            }
        }

        public Command Edit
        {
            get
            {
                return _editCommand ?? (_editCommand = new Command
                {
                    ExecuteAction = EditImpl,
                    CanExecuteFunc = () => SelectedItem != null && !SelectedItem.IsFixed
                });
            }
        }

        public Command Delete
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new Command
                {
                    ExecuteAction = DeleteImpl,
                    CanExecuteFunc = () => SelectedItem != null && !SelectedItem.IsFixed
                });
            }
        }

        public bool CanModify
        {
            get { return SelectedItem != null && !SelectedItem.IsFixed; }
        }

        public static ConvertersSummaryViewModel GetDemo()
        {
            var viewModel = new ConvertersSummaryViewModel();
            viewModel.UpdateConverters();

            return viewModel;
        }
    }
}
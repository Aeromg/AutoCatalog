using System;
using System.Collections.ObjectModel;
using System.Linq;
using AutoCatalogLib.Business;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Exchange.ExcelFormat;
using AutoCatalogWpf.Views;
using AutoCatalogWpf.Views.Rules;

namespace AutoCatalogWpf.ViewModels.Rules
{
    public class RulesSummaryViewModel : ViewModel
    {
        private Command _deleteCommand;
        private Command _editCommand;
        private RuleViewModel _selectedItem;

        public RulesSummaryViewModel()
        {
            Rules = new ObservableCollection<RuleViewModel>();
            UpdateRules();
            //Demo();
            SelectedItem = Rules.FirstOrDefault();
        }

        public ObservableCollection<RuleViewModel> Rules { get; set; }

        public RuleViewModel SelectedItem
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
                    CanExecuteFunc = () => SelectedItem != null
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
                    CanExecuteFunc = () => SelectedItem != null
                });
            }
        }

        public bool CanModify
        {
            get { return SelectedItem != null; }
        }

        private void Demo()
        {
            Rules.Add(new ExcelRuleViewModel
            {
                Name = "Простое имя",
                Description = "Описание. Длинное и максимально полное",
                Identificator = "id123"
            });
        }

        private void OnEditorClosed(object sender, EventArgs e)
        {
            App.Window.ModalClosed -= OnEditorClosed;
            UpdateRules();
        }

        private void UpdateRules()
        {
            var items = ImportRulesLocator.GetBehaviors().ToArray();
            Rules.Clear();
            var excels = items.Where(i => i is ExcelImportRule).Cast<ExcelImportRule>();
            foreach (var rule in ExcelRuleViewModel.FromModels(excels))
            {
                Rules.Add(rule);
            }
        }

        #region CRUD methods

        private void CreateImpl()
        {
            App.Window.ModalClosed += OnEditorClosed;
            App.Window.NavigateModal(new ExcelRuleEditPage());
        }

        private void EditImpl()
        {
            App.Window.ModalClosed += OnEditorClosed;
            App.Window.NavigateModal(new ExcelRuleEditPage((ExcelRuleViewModel) SelectedItem));
        }

        private void DeleteImpl()
        {
            ImportRulesLocator.Remove(SelectedItem.Identificator);
            Rules.Remove(SelectedItem);
        }

        #endregion
    }
}
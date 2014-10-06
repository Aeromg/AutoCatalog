using System;
using System.Collections.ObjectModel;
using System.Linq;
using AutoCatalogLib.Business;
using AutoCatalogLib.Exchange.ImportTargets;
using AutoCatalogWpf.Views.SourcesProfiles;

namespace AutoCatalogWpf.ViewModels.SourcesProfiles
{
    public class ImportTargetsSummaryViewModel : ViewModel
    {
        private Command _deleteCommand;
        private Command _editCommand;
        private ImportTargetViewModel _selectedItem;

        public ObservableCollection<ImportTargetViewModel> Targets { get; private set; }

        public ImportTargetsSummaryViewModel()
        {
            Targets = new ObservableCollection<ImportTargetViewModel>();
            UpdateItems();
        }

        private void Demo()
        {
            Targets.Add(new FileImportTargetViewModel()
            {
                FilePath = @"c:\Temp\masuma.xls",
                ImportRule = null
            });
            Targets.Add(new FileImportTargetViewModel()
            {
                FilePath = @"c:\Temp\tiss.xls",
                ImportRule = null
            });
            Targets.Add(new FileImportTargetViewModel()
            {
                FilePath = @"c:\abracadabra\file.xls",
                ImportRule = null
            });

            Targets.Add(new FileImportTargetViewModel()
            {
                FilePath = @"c:\import\*.xls",
                ImportRule = null
            });
            Targets.Add(new FileImportTargetViewModel()
            {
                FilePath = @"c:\*\file.xls",
                ImportRule = null
            });

            Targets.Add(new WebImportTargetViewModel()
            {
                Url = @"http://www.example.com/files/masuma.xls",
                ImportRule = null
            });
            Targets.Add(new WebImportTargetViewModel()
            {
                Url = @"http://files.domain.ru/download.aspx?f=abracadabra&type=xls",
                ImportRule = null
            });
        }

        public ImportTargetViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value)
                    return;

                //UpdateItems();

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
                    CanExecuteFunc = () => CanModify
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
                    CanExecuteFunc = () => CanModify
                });
            }
        }

        public bool CanModify
        {
            get { return SelectedItem != null; }
        }

        private void CreateImpl()
        {
            App.Window.NavigateModal(new FileTargetPage());
            App.Window.ModalClosed += WindowOnModalClosed;
        }

        private void WindowOnModalClosed(object sender, EventArgs eventArgs)
        {
            App.Window.ModalClosed -= WindowOnModalClosed;
            UpdateItems();
        }

        private void EditImpl()
        {
            if (!CanModify)
                return;

            App.Window.NavigateModal(new FileTargetPage((FileImportTargetViewModel)SelectedItem));
            App.Window.ModalClosed += WindowOnModalClosed;
        }

        private void DeleteImpl()
        {
            if (!CanModify)
                return;

            var item = SelectedItem;
            Targets.Remove(item);
            TargetsLocator.Remove(item.Guid);
        }

        private void UpdateItems()
        {
            var fileTargets = TargetsLocator.Targets.Where(t => t is FileSourceProfile).Cast<FileSourceProfile>().Select(t => new FileImportTargetViewModel(t));
            //var webTargets = TargetsLocator.Targets.Where(t => t is WebTarget).Cast<WebTarget>();

            /* foreach (var vm in fileTargets.Select(t => new FileImportTargetViewModel(t)))
                Targets.Add(vm); */

            Utils.CollectionUtils.UpdateObservableCollection(Targets, fileTargets);

            /* foreach (var vm in webTargets.Select(t => new FileImportTargetViewModel(t)))
                Targets.Add(vm); */
        }
    }
}
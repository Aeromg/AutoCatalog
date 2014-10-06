using System;
using AutoCatalogLib.Business;
using AutoCatalogLib.Exchange.ExcelFormat;
using AutoCatalogLib.Exchange.ImportTargets;
using Microsoft.Win32;

namespace AutoCatalogWpf.ViewModels.SourcesProfiles
{
    public class FileImportTargetViewModel : ImportTargetViewModel
    {
        public string FilePath { get; set; }

        private Command _saveCommand;
        public Command Save
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new Command
                {
                    CanExecuteFunc = ValidateForm,
                    ExecuteAction = SaveImpl
                });
            }
        }

        public Command BrowseFile
        {
            get
            {
                return new Command
                {
                    ExecuteAction = BrowseFileImpl
                };
            }
        }

        public Command Cancel
        {
            get
            {
                return new Command
                {
                    ExecuteAction = CancelImpl
                };
            }
        }

        public override string PresentationName
        {
            get { return FilePath; }
        }

        public override string PresentationDescription
        {
            get { return ImportRule != null ? ImportRule.Name : @""; }
        }

        public override TargetType TargetType
        {
            get { return TargetType.File; }
        }

        public FileImportTargetViewModel()
        {
            PropertyChanged += (sender, args) => Save.OnCanExecuteChanged();
            UpdateAvailableRules<ExcelImportRule>();
        }

        public FileImportTargetViewModel(FileSourceProfile rule) : this()
        {
            Model = rule;
            FillFromModel(this, rule);
        }

        private void BrowseFileImpl()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = @"Excel files|*.xls;*.xlsx";
            if (!dialog.ShowDialog().HasValue)
                return;

            FilePath = dialog.FileName;
        }

        private void CancelImpl()
        {
            RollBack();

            App.Window.NavigateModal(null);
        }

        private void RollBack()
        {
            if(Model == null)
                return;
            
            FillFromModel(this, (FileSourceProfile)Model);
        }

        private void SaveImpl()
        {
            var model = Model as FileSourceProfile;
            if (model == null)
            {
                model = new FileSourceProfile {Guid = Guid.NewGuid()};
            }
            model.FilePath = FilePath;
            model.TransactionIdentificator = Transaction;
            model.RuleIdentificatorString = ImportRule.Identificator;
            model.CleanBeforeImport = CleanBeforeImport;
            model.EmbedSource = EmbedSource;
            model.Distributor = Distributor;
            TargetsLocator.Set(model);

            App.Window.NavigateModal(null);
        }

        private bool ValidateForm()
        {
            return
                !String.IsNullOrEmpty(FilePath) &&
                !String.IsNullOrEmpty(Distributor) &&
                ImportRule != null &&
                !String.IsNullOrEmpty(Transaction);
        }

        public static void FillFromModel(FileImportTargetViewModel viewModel, FileSourceProfile model)
        {
            viewModel.Model = model;
            // viewModel.Guid = model.Guid;
            viewModel.Transaction = model.TransactionIdentificator;
            viewModel.FilePath = model.FilePath;
            viewModel.UpdateAvailableRules<ExcelImportRule>();
            viewModel.ImportRule = model.Rule;
            viewModel.CleanBeforeImport = model.CleanBeforeImport;
            viewModel.EmbedSource = model.EmbedSource;
            viewModel.Distributor = model.Distributor;
        }
    }
}
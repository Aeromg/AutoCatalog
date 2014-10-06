using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AutoCatalogLib.Business;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Exchange.ExcelFormat;

namespace AutoCatalogWpf.ViewModels.Rules
{
    public sealed class ExcelRuleViewModel : RuleViewModel
    {
        public ExcelRuleViewModel()
        {
            Defaults();
            PropertyChanged += OnPropertyChanged;
            Save.OnCanExecuteChanged();
        }

        private ExcelImportRule _model;

        public int? RowOffset { get; set; }
        public int? ColumnOffset { get; set; }
        public int? WorksheetIndex { get; set; }
        public bool UseCsvFile { get; set; }

        private IEnumerable<RuleFieldViewModel> Fields { get; set; }

        public bool IsNew
        {
            get { return Model == null; }
        }

        public RuleFieldViewModel NameField { get; set; }
        public RuleFieldViewModel BrandField { get; set; }
        public RuleFieldViewModel PartNumberField { get; set; }
        public RuleFieldViewModel PriceField { get; set; }
        public RuleFieldViewModel BalanceField { get; set; }
        public RuleFieldViewModel AnalogsField { get; set; }
        public RuleFieldViewModel DistributorField { get; set; }
        public RuleFieldViewModel CommentaryField { get; set; }


        private void Defaults()
        {
            var fields = new List<RuleFieldViewModel>();

            NameField = new RuleFieldViewModel
            {
                Name = "Name",
                GeneralizedType = GeneralizedType.String,
                Active = true,
                Required = true,
                BaseField = true,
                //Index = 0,
                Formatter = FormattersLocator.Search<string>().First()
            };
            fields.Add(NameField);

            BrandField = new RuleFieldViewModel
            {
                Name = "Brand",
                GeneralizedType = GeneralizedType.String,
                Active = true,
                Required = true,
                BaseField = false,
                //Index = 0,
                Formatter = FormattersLocator.Search<string>().First()
            };
            fields.Add(BrandField);

            PartNumberField = new RuleFieldViewModel
            {
                Name = "PartNumber",
                GeneralizedType = GeneralizedType.String,
                Active = true,
                Required = true,
                BaseField = true,
                //Index = 0,
                Formatter = FormattersLocator.Search<string>().First()
            };
            fields.Add(PartNumberField);

            PriceField = new RuleFieldViewModel
            {
                Name = "Price",
                GeneralizedType = GeneralizedType.Float,
                Active = true,
                Required = true,
                BaseField = true,
                //Index = 0,
                Formatter = FormattersLocator.Search<double>().First()
            };
            fields.Add(PriceField);

            BalanceField = new RuleFieldViewModel
            {
                Name = "Balance",
                GeneralizedType = GeneralizedType.Float,
                Active = true,
                Required = true,
                BaseField = false,
                //Index = 0,
                Formatter = FormattersLocator.Search<double>().First()
            };
            fields.Add(BalanceField);

            AnalogsField = new RuleFieldViewModel
            {
                Name = "Analogs",
                GeneralizedType = GeneralizedType.ArrayOfString,
                Active = true,
                Required = false,
                BaseField = false,
                //Index = 0,
                Formatter = FormattersLocator.Search<string[]>().First()
            };
            fields.Add(AnalogsField);

            Fields = fields;

            SubscrubeFieldsChanges();
        }

        private void SubscrubeFieldsChanges()
        {
            foreach (var field in Fields)
                field.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Save.OnCanExecuteChanged();
        }

        protected override void SaveImpl()
        {
            var model = Model as ExcelImportRule ?? new ExcelImportRule();

            model.CsvMode = UseCsvFile;
            model.Name = Name;
            model.Description = Description;
            model.Identificator = Identificator;
            model.RowOffset = RowOffset ?? 0;
            model.ColumnOffset = ColumnOffset ?? 0;
            model.WorksheetIndex = WorksheetIndex ?? 0;
            model.Columns = Fields.Select(f => new ExcelImportRule.Column
            {
                Active = f.Active,
                Index = f.Index ?? 0,
                Formatter = f.Formatter,
                Name = f.Name,
                Required = f.Required
            });

            ImportRulesLocator.SetBehavior(model);

            App.Window.NavigateModal(null);
        }

        protected override void CancelImpl()
        {
            RollBack();
            App.Window.NavigateModal(null);
        }

        protected override bool IsFormValid()
        {
            try
            {
                return
                    !String.IsNullOrEmpty(Identificator.Trim()) &&
                    !String.IsNullOrEmpty(Name.Trim()) &&
                    !String.IsNullOrEmpty(Description.Trim()) &&
                    RowOffset >= 0 &&
                    ColumnOffset >= 0 &&
                    WorksheetIndex > 0 &&
                    Fields.All(IsValidField);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IsValidField(RuleFieldViewModel field)
        {
            if (!field.Active)
                return true;

            if (field.Formatter == null)
                return false;

            if (!field.Index.HasValue || field.Index <= 0)
                return false;

            return true;
        }

        private void RollBack()
        {
            if (Model != null)
                FillViewModel(this, (ExcelImportRule)Model);
        }

        public static IEnumerable<ExcelRuleViewModel> FromModels(IEnumerable<ExcelImportRule> models)
        {
            foreach (var model in models)
            {
                var viewModel = new ExcelRuleViewModel();
                FillViewModel(viewModel, model);
                yield return viewModel;
            }
        }

        private static void FillViewModel(ExcelRuleViewModel viewModel, ExcelImportRule model)
        {
            viewModel._model = model;

            viewModel.Identificator = model.Identificator;
            viewModel.Name = model.Name;
            viewModel.Description = model.Description;
            viewModel.RowOffset = model.RowOffset;
            viewModel.ColumnOffset = model.ColumnOffset;
            viewModel.UseCsvFile = model.CsvMode;
            viewModel.WorksheetIndex = model.WorksheetIndex;

            FillFieldViewModel(viewModel.NameField,
                model.Columns.FirstOrDefault(c => c.Name == viewModel.NameField.Name));

            FillFieldViewModel(viewModel.BrandField,
                model.Columns.FirstOrDefault(c => c.Name == viewModel.BrandField.Name));

            FillFieldViewModel(viewModel.PartNumberField,
                model.Columns.FirstOrDefault(c => c.Name == viewModel.PartNumberField.Name));

            FillFieldViewModel(viewModel.PriceField,
                model.Columns.FirstOrDefault(c => c.Name == viewModel.PriceField.Name));

            FillFieldViewModel(viewModel.BalanceField,
                model.Columns.FirstOrDefault(c => c.Name == viewModel.BalanceField.Name));

            FillFieldViewModel(viewModel.AnalogsField,
                model.Columns.FirstOrDefault(c => c.Name == viewModel.AnalogsField.Name));
            /*
            FillFieldViewModel(viewModel.DistributorField, model.Columns.FirstOrDefault(c => c.Name == viewModel.DistributorField.Name));
            FillFieldViewModel(viewModel.CommentaryField, model.Columns.FirstOrDefault(c => c.Name == viewModel.CommentaryField.Name)); */

            viewModel.RaisePropertyChanged("Model");
            viewModel.RaisePropertyChanged("IsNew");
        }

        private static void FillFieldViewModel(RuleFieldViewModel viewModel, ExcelImportRule.Column model)
        {
            if (model == null)
                return;

            viewModel.UpdateCompatableFormatters();
            viewModel.Formatter = FormattersLocator.Get(model.Formatter.Guid);
            viewModel.Active = model.Active;
            viewModel.Required = model.Required;
            viewModel.Index = model.Index;
        }

        protected override IRule GetModel()
        {
            return _model;
        }
    }
}
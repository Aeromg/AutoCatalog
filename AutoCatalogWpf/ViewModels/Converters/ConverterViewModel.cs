using System;
using AutoCatalogLib.Business;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Exchange.ValueFormatters;

namespace AutoCatalogWpf.ViewModels.Converters
{
    public class ConverterViewModel : ViewModel
    {
        private Command _cancelCommand;
        private Command _saveCommand;

        public ConverterViewModel()
        {
            GeneralizedType = GeneralizedType.Unknown;
            Guid = Guid.NewGuid();

            PropertyChanged += (sender, args) =>
            {
                if (Save != null)
                    Save.OnCanExecuteChanged();
            };
        }

        private IFormatter Model { get; set; }

        public Guid Guid { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserScript { get; set; }
        public GeneralizedType GeneralizedType { get; set; }

        public bool IsFixed
        {
            get { return Model != null && !(Model is JavaScriptValueFormatter); }
        }

        public Command Cancel
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new Command
                {
                    ExecuteAction = CancelImpl
                });
            }
        }

        public Command Save
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new Command
                {
                    ExecuteAction = SaveImpl,
                    CanExecuteFunc = IsFormValid
                });
            }
        }

        private void CancelImpl()
        {
            if (Model != null)
                RollBack();

            App.Window.NavigateModal(null);
        }

        private void RollBack()
        {
            FillViewModel(this, Model);
        }

        private void SaveImpl()
        {
            var model = Model as JavaScriptValueFormatter;

            if (model == null)
                model = new JavaScriptValueFormatter();

            model.Guid = Guid == Guid.Empty ? Guid.NewGuid() : Guid;
            model.Name = Name;
            model.Description = Description;
            model.Script = UserScript;
            model.Type = GeneralizedTypes.GetConcreteType(GeneralizedType);

            FormattersLocator.Add(model);

            App.Window.NavigateModal(null);
        }

        private bool IsFormValid()
        {
            return
                //Guid != Guid.Empty &&
                !String.IsNullOrEmpty(Name) &&
                !String.IsNullOrEmpty(Description) &&
                !String.IsNullOrEmpty(UserScript);
        }

        public static ConverterViewModel FromModel(IFormatter formatter)
        {
            var viewModel = new ConverterViewModel();
            FillViewModel(viewModel, formatter);
            return viewModel;
        }

        private static void FillViewModel(ConverterViewModel viewModel, IFormatter model)
        {
            viewModel.Guid = model.Guid;
            viewModel.Name = model.Name;
            viewModel.Description = model.Description;
            viewModel.Model = model;
            viewModel.GeneralizedType = GeneralizedTypes.GetGeneralizedTypeValue(model.Type);
            viewModel.UserScript = (model is JavaScriptValueFormatter)
                ? ((JavaScriptValueFormatter) model).Script
                : String.Empty;
        }
    }
}
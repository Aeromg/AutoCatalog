using System;
using AutoCatalogLib.Exchange;

namespace AutoCatalogWpf.ViewModels.Rules
{
    public abstract class RuleViewModel : ViewModel
    {
        private Command _cancelCommand;
        private Command _saveCommand;

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

        public IRule Model { get { return GetModel(); } }

        public string Identificator { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        protected abstract void SaveImpl();
        protected abstract void CancelImpl();
        protected abstract bool IsFormValid();

        protected abstract IRule GetModel();
    }
}
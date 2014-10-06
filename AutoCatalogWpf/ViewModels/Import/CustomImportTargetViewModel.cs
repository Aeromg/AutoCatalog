using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Business;
using AutoCatalogLib.Exchange;

namespace AutoCatalogWpf.ViewModels.Import
{
    public class CustomImportTargetViewModel : ViewModel
    {
        private IEnumerable<IRule> _availableRules;

        public IEnumerable<IRule> AvailableRules
        {
            get { return _availableRules ?? (_availableRules = ImportRulesLocator.GetBehaviors()); }
        }

        public string FilePath { get; set; }
        public string FilePathEditable { get; set; }

        public string Identificator { get; set; }
        public string IdentificatorEditable { get; set; }

        public IRule Rule { get; set; }
        public IRule RuleEditable { get; set; }

        public string RulePresentation
        {
            get { return Rule.Identificator + " (" + Rule.Name + ")"; }
        }

        public void Save()
        {
            FilePath = FilePathEditable;
            Rule = RuleEditable;
            RaisePropertyChanged("RulePresentation");
        }

        public void RollBack()
        {
            FilePathEditable = FilePath;
            RuleEditable = Rule;
            RaisePropertyChanged("RulePresentation");
        }

        internal bool CheckForm()
        {
            return !String.IsNullOrEmpty(FilePathEditable) && RuleEditable != null;
        }
    }
}

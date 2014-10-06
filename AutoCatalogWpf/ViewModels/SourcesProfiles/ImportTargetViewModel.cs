using System;
using System.Collections.ObjectModel;
using System.Linq;
using AutoCatalogLib.Business;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Exchange.ImportTargets;

namespace AutoCatalogWpf.ViewModels.SourcesProfiles
{
    public abstract class ImportTargetViewModel : ViewModel
    {
        protected ISourceProfile Model;

        public Guid Guid
        {
            get { return Model == null ? Guid.Empty : Model.Guid; }
        }

        public IRule ImportRule { get; set; }

        public string Transaction { get; set; }
        public string Distributor { get; set; }
        public bool CleanBeforeImport { get; set; }
        public bool EmbedSource { get; set; }

        public ObservableCollection<IRule> AvailableRules { get; set; }
        public abstract string PresentationName { get; }
        public abstract string PresentationDescription { get; }
        public abstract TargetType TargetType { get; }

        protected void UpdateAvailableRules<TRule>() where TRule : IRule
        {
            if(AvailableRules == null)
                AvailableRules = new ObservableCollection<IRule>();

            var rules = ImportRulesLocator.GetBehaviors().Where(r => r is TRule).ToArray();

            if (AvailableRules.Count == 0)
            {
                foreach (var rule in rules)
                {
                    AvailableRules.Add(rule);
                }
                return;
            }

            var toDelete = AvailableRules.Where(r => !rules.Contains(r)).ToArray();
            var toAdd = rules.Where(r => !AvailableRules.Contains(r)).ToArray();

            foreach (var rule in toDelete)
                AvailableRules.Remove(rule);

            foreach (var rule in toAdd)
                AvailableRules.Add(rule);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Business;

namespace AutoCatalogLib.Exchange.ImportTargets
{
    public abstract class SourceProfile : ISourceProfile
    {
        public string RuleIdentificatorString { get; set; }

        private Guid? _guid;
        public Guid Guid
        {
            get { return _guid ?? Guid.Empty; }
            set
            {
                if (_guid.HasValue && _guid.Value != value)
                    throw new ReadOnlyException();

                _guid = value;
            }
        }

        public bool CleanBeforeImport { get; set; }
        public bool EmbedSource { get; set; }

        private ISource _source;
        public ISource Source
        {
            get { 
                if(_source != null)
                    return _source;
                
                _source = SourceLocator.GetSourceByLocation(GetLocation());
                return _source;
            }
        }

        private IRule _rule;
        public IRule Rule
        {
            get
            {
                if (_rule != null && _rule.Identificator == RuleIdentificatorString)
                    return _rule;

                CheckRuleIdentificatorString();
                _rule = ImportRulesLocator.GetBehavior(RuleIdentificatorString);
                return _rule;
            }
        }

        public string TransactionIdentificator { get; set; }

        public string Distributor { get; set; }

        private void CheckRuleIdentificatorString()
        {
            if (String.IsNullOrEmpty(RuleIdentificatorString))
                throw new Exception("Empty rule identificator string");
        }

        protected abstract string GetLocation();
    }
}

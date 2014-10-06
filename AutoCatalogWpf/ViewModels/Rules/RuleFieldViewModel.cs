using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoCatalogLib.Business;
using AutoCatalogLib.Exchange;
using AutoCatalogWpf.Utils;

namespace AutoCatalogWpf.ViewModels.Rules
{
    public class RuleFieldViewModel : ViewModel
    {
        private bool _active;
        private bool _required;

        public string Name { get; set; }
        private GeneralizedType _generalizedType;

        public GeneralizedType GeneralizedType
        {
            get { return _generalizedType; }
            set
            {
                if(_generalizedType == value)
                    return;

                _generalizedType = value;
                UpdateCompatableFormatters();
            }
        }
        public IFormatter Formatter { get; set; }

        public bool Active
        {
            get { return BaseField || _active; }
            set { _active = value; }
        }

        public bool Required
        {
            get { return BaseField || _required; }
            set { _required = value; }
        }

        public int? Index { get; set; }

        public bool BaseField { get; set; }

        public bool FreeField
        {
            get { return !BaseField; }
        }

        public ObservableCollection<IFormatter> CompatableFormatters { get; set; }

        public void UpdateCompatableFormatters()
        {
            if (CompatableFormatters == null)
                    CompatableFormatters = new ObservableCollection<IFormatter>();

            var formatters = FormattersLocator.Search(GeneralizedTypes.GetConcreteType(GeneralizedType)).ToArray();

            if (CompatableFormatters.Count == 0)
            {
                foreach (var formatter in formatters)
                    CompatableFormatters.Add(formatter);

                return;
            }

            CollectionUtils.UpdateObservableCollection(CompatableFormatters, formatters);
        }
    }
}
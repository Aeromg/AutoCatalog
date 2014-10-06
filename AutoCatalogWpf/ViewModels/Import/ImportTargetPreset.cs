using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Exchange.ImportTargets;

namespace AutoCatalogWpf.ViewModels.Import
{
    public class ImportTargetPreset : ViewModel
    {
        public bool IsSelected { get; set; }

        public ISourceProfile Target { get; set; }

        public string PresentationName
        {
            get
            {
                return ((SourceProfile)Target).RuleIdentificatorString + " (" + ((SourceProfile)Target).Rule.Name + ")";
            }
        }

        public string PresentationDetails
        {
            get
            {
                if (Target is FileSourceProfile)
                    return ((FileSourceProfile) Target).FilePath;

                if (Target is WebSourceProfile)
                    return ((WebSourceProfile)Target).Url;

                return @"Неизвестный источник";
            }
        }
    }
}

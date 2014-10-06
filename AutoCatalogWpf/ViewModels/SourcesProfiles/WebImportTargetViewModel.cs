using AutoCatalogLib.Exchange.ImportTargets;

namespace AutoCatalogWpf.ViewModels.SourcesProfiles
{
    internal class WebImportTargetViewModel : ImportTargetViewModel
    {
        public string Url { get; set; }

        public override string PresentationName
        {
            get { return Url; }
        }

        public override string PresentationDescription
        {
            get { return ImportRule != null ? ImportRule.Name : @""; }
        }

        public override TargetType TargetType
        {
            get { return TargetType.Web; }
        }
    }
}
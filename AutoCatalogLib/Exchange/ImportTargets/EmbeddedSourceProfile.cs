using AutoCatalogLib.Business;

namespace AutoCatalogLib.Exchange.ImportTargets
{
    class EmbeddedSourceProfile : SourceProfile
    {
        public string BlobName { get; set; }

        protected override string GetLocation()
        {
            return SourceLocator.EmbeddedProtocol + BlobName;
        }
    }
}
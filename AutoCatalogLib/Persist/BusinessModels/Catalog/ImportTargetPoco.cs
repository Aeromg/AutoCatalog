using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Exchange.ImportTargets;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Persist.BusinessModels.Catalog
{
    public class ImportTargetPoco : Entity, IBridgeable<ISourceProfile, ImportTargetPoco>
    {
        private Bridge<ISourceProfile, ImportTargetPoco> _bridge;

        public Guid Guid { get; set; }
        public TargetType TargetType { get; set; }
        public string Target { get; set; }
        public string RuleIdentificator { get; set; }
        public string TransactionIdentificator { get; set; }
        public bool CleanBeforeImport { get; set; }
        public bool EmbedSource { get; set; }
        public string Distributor { get; set; }

        public Bridge<ISourceProfile, ImportTargetPoco> Bridge
        {
            get { return _bridge ?? (_bridge = new ImportTargetBridge(this)); }
        }
    }
}

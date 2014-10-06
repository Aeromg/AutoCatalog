using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Business;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Exchange.ImportTargets
{
    public class FileSourceProfile : SourceProfile
    {
        public string FilePath { get; set; }

        public bool IsWildcard
        {
            get { return FilePath.Contains("*") || FilePath.Contains("?"); }
        }

        protected override string GetLocation()
        {
            return SourceLocator.LocalPathProtocol + FilePath;
        }
    }
}

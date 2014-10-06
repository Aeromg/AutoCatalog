using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Business;

namespace AutoCatalogLib.Exchange.ImportTargets
{
    public class WebSourceProfile : SourceProfile
    {
        public string Url { get; set; }

        protected override string GetLocation()
        {
            return SourceLocator.WebProtocol + Url;
        }
    }
}

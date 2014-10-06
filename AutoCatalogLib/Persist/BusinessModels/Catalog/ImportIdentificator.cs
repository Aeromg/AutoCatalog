using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Persist.BusinessModels.Catalog
{
    public class ImportIdentificator : Entity
    {
        public string SourceLocation { get; set; }
        public string Identificator { get; set; }
        public string Distributor { get; set; }
        public DateTime DateTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCatalogLib.Exchange
{
    public class RecordRow : Dictionary<string, object>
    {
        public ISource Source { get; set; }
        public string SourceArgument { get; set; }

        public RecordRow(int columns) : base(columns) { }
    }
}

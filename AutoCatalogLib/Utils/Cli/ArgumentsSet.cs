using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCatalogLib.Utils.Cli
{
    public class ArgumentsSet
    {
        private readonly Argument[] _arguments;

        public ArgumentsSet(IEnumerable<Argument> arguments)
        {
            _arguments = arguments.ToArray();
        }

        public bool Has(ArgumentHeader header)
        {
            return _arguments.Any(a => a.Header == header);
        }

        public Argument First(ArgumentHeader header)
        {
            return _arguments.First(a => a.Header == header);
        }

        public IEnumerable<Argument> All(ArgumentHeader header)
        {
            return _arguments.Where(a => a.Header == header);
        }

        public IEnumerable<Argument> All()
        {
            return _arguments;
        }
    }
}

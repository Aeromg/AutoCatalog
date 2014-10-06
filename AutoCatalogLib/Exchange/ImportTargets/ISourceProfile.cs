using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCatalogLib.Exchange.ImportTargets
{
    public interface ISourceProfile
    {
        Guid Guid { get; }
        ISource Source { get; }
        IRule Rule { get; }
        string TransactionIdentificator { get; }
        string Distributor { get; }
        bool CleanBeforeImport { get; }
        bool EmbedSource { get; }
    }
}

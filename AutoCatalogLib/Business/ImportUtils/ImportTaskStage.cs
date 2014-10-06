using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCatalogLib.Business.ImportUtils
{
    public enum ImportTaskStage
    {
        Awaiting = 0,
        Prepare = 1,
        Import = 2,
        PostProcess = 3,
        Finished = 4,
        Interrupted = 5
    }
}

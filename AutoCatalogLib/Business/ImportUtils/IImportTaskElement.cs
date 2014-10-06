using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCatalogLib.Business.ImportUtils
{
    public interface IImportTaskElement : IImportTask
    {
        void StartDriven();

        void Prepare();
        Task PrepareAsync();

        void Import();
        Task ImportAsync();

        void PostProcess();
        Task PostProcessAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCatalogLib.Business.ImportUtils
{
    public interface IImportTask
    {
        ImportTaskStage Stage { get; }
        int ImportedRecordsCount { get; }
        int TotalRecordsCount { get; }
        bool IsInterrupted { get; }
        bool IsFinished { get; }
        bool IsStarted { get; }
        bool IsImported { get; }
        bool IsPrepared { get; }
        bool IsPostProcessed { get; }
        bool IsBusy { get; }

        event EventHandler<ImportTaskStage> StageChanged;
        event EventHandler ProgressChanged;
        event EventHandler Started;
        event EventHandler Finished;
        event EventHandler Interrupted;

        void Start();
        Task StartAsync();

        void Interrupt();
        Task InterruptAsync();
    }
}

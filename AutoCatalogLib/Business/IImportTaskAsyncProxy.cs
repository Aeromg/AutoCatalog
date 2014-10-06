using System;

namespace AutoCatalogLib.Business
{
    public interface IImportTaskAsyncProxy
    {
        int TotalCount { get; }
        int ProgressCount { get; }
        bool IsProgress { get; }
        bool IsDone { get; }
        bool IsInterrupted { get; }
        event EventHandler ProgressChanged;
        event EventHandler StartReading;
        event EventHandler Finish;
        event EventHandler Interrupted;
        void Start();
        void Stop();
    }
}
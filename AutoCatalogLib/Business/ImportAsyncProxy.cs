using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoCatalogLib.Utils;

namespace AutoCatalogLib.Business
{
    public class ImportAsyncProxy : IImportTaskAsyncProxy
    {
        private readonly BufferedSourceReader _task;
        private readonly BufferedItemsWriter _transaction;

        private volatile int _recordsCount;
        //private volatile int _recordsReaded;
        private volatile int _recordsWrited;

        private volatile bool _interrupted;
        private Task _importTask;
        private Task _observerTask;

        public int TotalCount
        {
            get { return _task.TotalCount; }
        }

        public int ProgressCount
        {
            get { return _recordsWrited; }
        }

        public bool IsProgress { get; private set; }
        public bool IsDone { get; private set; }
        public bool IsInterrupted { get { return _interrupted; } }

        public event EventHandler ProgressChanged;
        public event EventHandler StartReading;
        public event EventHandler Finish;
        public event EventHandler Interrupted;

        internal ImportAsyncProxy(BufferedSourceReader task, BufferedItemsWriter transaction)
        {
            _task = task;
            _transaction = transaction;
        }

        public void Start()
        {
            if (IsInterrupted || IsDone)
                throw new Exception(@"Процесс был прерван ранее");

            if (IsProgress)
                throw new Exception(@"Процесс уже запущен");

            IsProgress = true;

            _importTask = new Task(ImportTask);
            _observerTask = new Task(ObserverTask);

            _importTask.Start();
            _observerTask.Start();
        }

        public void Stop()
        {
            if (IsInterrupted || IsDone)
                throw new Exception(@"Процесс был прерван ранее");

            if (!IsProgress)
                throw new Exception(@"Процесс не запущен");

            _interrupted = true;
            OnInterrupted();
        }

        private void ImportTask()
        {
            try
            {
                _task.Open();
                
                _transaction.WriteItems(_task.ReadBuffer());
                IsDone = true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                _interrupted = true;
                OnInterrupted();
            }
            IsProgress = false;
            OnFinish();
        }

        private void ObserverTask()
        {
            bool recievedFirstElement = false;
            int lastWrited = 0;
            while (!IsInterrupted && !IsDone)
            {
                //_recordsReaded = _task.RecordsReadedCount;
                _recordsWrited = _transaction.WritenCount;
                Thread.Sleep(200);
                if (lastWrited != _recordsWrited)
                {
                    if (!recievedFirstElement)
                    {
                        OnStartReading();
                        recievedFirstElement = true;
                    }

                    OnProgressChanged();
                    lastWrited = _recordsWrited;
                }
            }
        }

        private void OnProgressChanged()
        {
            var handler = ProgressChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnFinish()
        {
            var handler = Finish;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnInterrupted()
        {
            var handler = Interrupted;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnStartReading()
        {
            var handler = StartReading;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoCatalogLib.Business.ImportUtils
{
    public abstract class ImportTaskElement : IImportTaskElement
    {
        private ImportTaskStage _stage;
        private int _importedRecordsCount;
        private int _totalRecordsCount;
        private bool _isImported;
        private bool _isInterrupted;
        private bool _isFinished;
        private bool _isStarted;
        private bool _isPrepared;
        private bool _isPostProcessed;
        private int _statusPollingInterval = 100;   // default is 100 msec

        public ImportTaskStage Stage
        {
            get { return _stage; }
            private set
            {
                if (value == _stage)
                    return;

                _stage = value;
                OnStageChanged();
            }
        }
        public int ImportedRecordsCount
        {
            get { return _importedRecordsCount; }
            private set
            {
                if (value == _importedRecordsCount)
                    return;

                _importedRecordsCount = value;
                OnProgressChanged();
            }
        }
        public int TotalRecordsCount
        {
            get { return _totalRecordsCount; }
            private set
            {
                if (value == _totalRecordsCount)
                    return;

                _totalRecordsCount = value;
                OnProgressChanged();
            }
        }
        public bool IsImported
        {
            get { return _isImported; }
            private set
            {
                if (value == _isImported)
                    return;

                _isImported = value;
                OnProgressChanged();
            }
        }
        public bool IsInterrupted
        {
            get { return _isInterrupted; }
            private set
            {
                if (value == _isInterrupted)
                    return;

                _isInterrupted = value;
                OnInterrupted();
            }
        }
        public bool IsFinished
        {
            get { return _isFinished; }
            private set
            {
                if (value == _isFinished)
                    return;

                _isFinished = value;
                OnFinished();
            }
        }
        public bool IsStarted
        {
            get { return _isStarted; }
            private set
            {
                if (value == _isStarted)
                    return;

                _isStarted = value;
                OnStarted();
            }
        }
        public bool IsPrepared
        {
            get { return _isPrepared; }
            private set
            {
                if (value == _isPrepared)
                    return;

                _isPrepared = value;
            }
        }
        public bool IsPostProcessed
        {
            get { return _isPostProcessed; }
            private set
            {
                if (value == _isPostProcessed)
                    return;

                _isPostProcessed = value;
            }
        }
        public bool IsBusy
        {
            get { return Stage != ImportTaskStage.Awaiting; }
        }

        public int StatusPollingInterval
        {
            get { return _statusPollingInterval; }
            set
            {
                if(value < 1)
                    throw new ArgumentOutOfRangeException();

                _statusPollingInterval = value;
            }
        }

        public event EventHandler<ImportTaskStage> StageChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler Started;
        public event EventHandler Finished;
        public event EventHandler Interrupted;

        public virtual void Start()
        {
            if (IsBusy)
                throw new InvalidOperationException(@"Операция выполяется");

            if (IsStarted)
                throw new InvalidOperationException(@"Задача уже запущена");

            if (IsFinished || IsInterrupted)
                throw new InvalidOperationException(@"Задача завершена");

            IsStarted = true;

            GoStages();
        }

        public Task StartAsync()
        {
            return Task.Factory.StartNew(Start);
        }

        private void GoStages()
        {
            Prepare();
            Import();
            PostProcess();

            Stage = ImportTaskStage.Finished;
            IsFinished = true;
        }

        public virtual void StartDriven()
        {
            if(IsBusy)
                throw new InvalidOperationException(@"Операция выполяется");

            if (IsStarted)
                throw new InvalidOperationException(@"Задача уже запущена");

            if (IsFinished || IsInterrupted)
                throw new InvalidOperationException(@"Задача завершена");

            IsStarted = true;
        }

        public void Interrupt()
        {
            if (!IsStarted)
                throw new InvalidOperationException(@"Задача не запущена");

            if (IsFinished || IsInterrupted)
                throw new InvalidOperationException(@"Задача уже завершена");

            Stage = ImportTaskStage.Interrupted;
            InterruptRoutine();
            IsInterrupted = true;
            Stage = ImportTaskStage.Finished;
            IsFinished = true;
        }

        public Task InterruptAsync()
        {
            return Task.Factory.StartNew(Interrupt);
        }

        public void Prepare()
        {
            if (IsBusy)
                throw new InvalidOperationException(@"Операция выполяется");

            if (!IsStarted)
                throw new InvalidOperationException(@"Задача не запущена");

            if (IsPrepared)
                throw new InvalidOperationException(@"Стадия подготовки уже пройдена");

            if (IsFinished || IsInterrupted)
                throw new InvalidOperationException(@"Задача уже завершена");

            Stage = ImportTaskStage.Prepare;
            PrepareRoutine();
            UpdateStatus();
            Stage = ImportTaskStage.Awaiting;
            IsPrepared = true;
        }

        public Task PrepareAsync()
        {
            return Task.Factory.StartNew(Prepare);
        }

        public void Import()
        {
            if (IsBusy)
                throw new InvalidOperationException(@"Операция выполяется");

            if (!IsStarted)
                throw new InvalidOperationException(@"Задача не запущена");

            if (IsImported)
                throw new InvalidOperationException(@"Этап импорта уже пройден");

            if (IsFinished || IsInterrupted)
                throw new InvalidOperationException(@"Задача уже завершена");

            Stage = ImportTaskStage.Import;
            StartStatusPolling();
            ImportRoutine();
            Stage = ImportTaskStage.Awaiting;
            IsImported = true;
        }

        public Task ImportAsync()
        {
            return Task.Factory.StartNew(Import);
        }

        public void PostProcess()
        {
            if (IsBusy)
                throw new InvalidOperationException(@"Операция выполяется");

            if (!IsStarted)
                throw new InvalidOperationException(@"Задача не запущена");

            if (IsPostProcessed)
                throw new InvalidOperationException(@"Стадия завершения уже пройдена");

            if (IsFinished || IsInterrupted)
                throw new InvalidOperationException(@"Задача уже завершена");

            Stage = ImportTaskStage.PostProcess;
            PostProcessRoutine();
            Stage = ImportTaskStage.Awaiting;
            IsPostProcessed = true;
        }

        public Task PostProcessAsync()
        {
            return Task.Factory.StartNew(PostProcess);
        }

        private void StartStatusPolling()
        {
            Task.Factory.StartNew(StatusPollingRoutine);
        }

        protected virtual void StatusPollingRoutine()
        {
            while (Stage == ImportTaskStage.Import && !IsFinished && !IsInterrupted)
            {
                UpdateStatus();
                Thread.Sleep(StatusPollingInterval);
            }
        }

        private void UpdateStatus()
        {
            TotalRecordsCount = GetTotalRecordsCount();
            ImportedRecordsCount = GetImportedRecordsCount();
        }

        protected abstract void PrepareRoutine();

        protected abstract void ImportRoutine();

        protected abstract void PostProcessRoutine();

        protected abstract void InterruptRoutine();

        protected abstract int GetTotalRecordsCount();

        protected abstract int GetImportedRecordsCount();
        
        #region Event methods

        protected virtual void OnStageChanged()
        {
            var handler = StageChanged;
            if (handler != null)
                handler(this, Stage);
        }

        protected virtual void OnProgressChanged()
        {
            var handler = ProgressChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnStarted()
        {
            var handler = Started;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnFinished()
        {
            var handler = Finished;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnInterrupted()
        {
            var handler = Interrupted;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

    }
}

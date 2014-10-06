using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AutoCatalogLib.Business;
using AutoCatalogLib.Business.ImportUtils;

namespace AutoCatalogWpf.ViewModels.Import
{
    public class ImportProgressViewModel : ViewModel
    {
        public int TotalCount { get; private set; }
        public int ProgressCount { get; private set; }
        public float Progress { get; private set; }

        public bool IsDone { get; set; }
        public bool IsProgress { get; private set; }
        public bool IsInterrupted { get; private set; }
        public bool IsFinished { get; private set; }
        public bool IsPreparing { get; private set; }
        public string StatePresentation { get; private set; }

        private Command _closePageCommand;
        public Command ClosePage
        {
            get
            {
                return _closePageCommand ?? (_closePageCommand = new Command
                {
                    ExecuteAction = CloseImpl
                });
            }
        }

        private IImportTask _progressProxy;

        public ImportProgressViewModel(IImportTask progressProxy)
        {
            _progressProxy = progressProxy;

            _progressProxy.StageChanged += (sender, args) => Dispatcher.CurrentDispatcher.Invoke(TaskStageChanged);
            _progressProxy.ProgressChanged += (sender, args) => Dispatcher.CurrentDispatcher.Invoke(TaskProgressChanged);
            _progressProxy.Interrupted += (sender, args) => Dispatcher.CurrentDispatcher.Invoke(TaskOnInterrupted);
            _progressProxy.Started += (sender, args) => Dispatcher.CurrentDispatcher.Invoke(TaskStarted);
            _progressProxy.Finished += (sender, args) => Dispatcher.CurrentDispatcher.Invoke(TaskFinished);
        }

        private void TaskFinished()
        {
            IsFinished = _progressProxy.IsFinished;
            IsDone = true;
        }

        private void TaskStarted()
        {
            // IsProgress = _progressProxy.IsStarted;
        }

        private void TaskOnInterrupted()
        {
            IsInterrupted = _progressProxy.IsInterrupted;
            IsDone = true;
        }

        private void TaskProgressChanged()
        {
            TotalCount = _progressProxy.TotalRecordsCount;
            ProgressCount = _progressProxy.ImportedRecordsCount;
            Progress = ((float)ProgressCount/TotalCount) * 100;
        }

        private void TaskStageChanged()
        {
            var stage = _progressProxy.Stage;
            switch (stage)
            {
                case ImportTaskStage.Awaiting:
                    StatePresentation = "ожидание";
                    IsPreparing = true;
                    IsProgress = false;
                    break;
                case ImportTaskStage.Prepare:
                    StatePresentation = "подготовка";
                    IsPreparing = true;
                    IsProgress = false;
                    break;
                case ImportTaskStage.Import:
                    StatePresentation = "импорт";
                    IsPreparing = false;
                    IsProgress = true;
                    break;
                case ImportTaskStage.PostProcess:
                    StatePresentation = "завершение";
                    IsPreparing = true;
                    IsProgress = false;
                    break;
                case ImportTaskStage.Finished:
                    StatePresentation = "завершено";
                    IsPreparing = false;
                    IsProgress = true;
                    break;
                case ImportTaskStage.Interrupted:
                    StatePresentation = "прервано";
                    IsPreparing = false;
                    IsProgress = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal void Start()
        {
            _progressProxy.StartAsync();
        }

        private void CloseImpl()
        {
            if (IsDone)
            {
                App.Window.NavigateModal(null);
                return;
            }

            if (MessageBox.Show("Импортированные записи не будут удалены.\nПрервать процесс?", "Импорт", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _progressProxy.InterruptAsync();
                App.Window.NavigateModal(null);
            }
                
        }
    }
}

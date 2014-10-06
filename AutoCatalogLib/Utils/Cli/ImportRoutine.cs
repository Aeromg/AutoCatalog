using System;
using System.Threading;
using AutoCatalogLib.Business;
using AutoCatalogLib.Business.ImportUtils;
using AutoCatalogLib.Exchange.ImportTargets;

namespace AutoCatalogLib.Utils.Cli
{
    class ImportRoutine : Routine
    {
        private IImportTask _task;

        private volatile EventWaitHandle _taskWaitHandle;

        public override void Run()
        {
            var targetPath = Arguments.First(ArgumentsDictionary.Import).Value;
            var ruleId = Arguments.First(ArgumentsDictionary.RuleId).Value;
            var transactionId = Arguments.First(ArgumentsDictionary.ImportId).Value;

            var target = new FileSourceProfile
            {
                FilePath = targetPath,
                TransactionIdentificator = transactionId,
                RuleIdentificatorString = ruleId
            };

            Console.WriteLine(@"������ �� ������������� ���������");
            Console.WriteLine(@"��������: " + target.Source.Location);
            Console.WriteLine(@"�������: " + target.Rule.Name + " (" + target.Rule.Identificator + ")");
            Console.WriteLine(@"�������������: " + target.TransactionIdentificator);

            _task = ImportToolkit.CreateImportTask(target);
            _task.StageChanged += TaskStageChanged;
            _task.ProgressChanged += TaskProgressChanged;
            _task.Interrupted += TaskOnInterrupted;
            _task.Started += TaskStarted;
            _task.Finished += TaskFinished;

            _taskWaitHandle = new AutoResetEvent(false);
            _task.Start();
            //Console.WriteLine("����������");

            _taskWaitHandle.WaitOne();
        }

        private void TaskFinished(object sender, EventArgs e)
        {
            Console.WriteLine(@"���������");
            _taskWaitHandle.Set();
        }

        private void TaskStarted(object sender, EventArgs e)
        {
            Console.WriteLine(@"������ �������");
        }

        private void TaskOnInterrupted(object sender, EventArgs e)
        {
            Console.WriteLine(@"��������");
            _taskWaitHandle.Set();
        }

        private void TaskProgressChanged(object sender, EventArgs e)
        {
            Console.CursorLeft = 0;
            Console.Write(@"                           ");
            Console.CursorLeft = 0;
            Console.Write(@"������������� {0} �� {1}...", _task.ImportedRecordsCount, _task.TotalRecordsCount);
        }

        private void TaskStageChanged(object sender, ImportTaskStage e)
        {
            switch (e)
            {
                case ImportTaskStage.Awaiting:
                    Console.WriteLine("��������");
                    break;
                case ImportTaskStage.Prepare:
                    Console.WriteLine("����������");
                    break;
                case ImportTaskStage.Import:
                    Console.WriteLine("������");
                    break;
                case ImportTaskStage.PostProcess:
                    Console.WriteLine("���������");
                    break;
                case ImportTaskStage.Finished:
                    Console.WriteLine("���������");
                    break;
                case ImportTaskStage.Interrupted:
                    Console.WriteLine("��������");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("e");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCatalogLib.Business.ImportUtils
{
    class ImportTaskMultiElement : ImportTaskElement
    {
        private readonly List<IImportTaskElement> _elements = new List<IImportTaskElement>();

        public IEnumerable<IImportTaskElement> Elements
        {
            get { return _elements.AsEnumerable(); }
        }

        public ImportTaskMultiElement(IEnumerable<IImportTaskElement> elements = null)
        {
            if (elements != null)
            {
                foreach (var element in elements)
                    AddElement(element);
            }
        }

        public void AddElement(IImportTaskElement element)
        {
            _elements.Add(element);
        }

        public void RemoveElement(IImportTaskElement element)
        {
            _elements.Remove(element);
        }

        public override void Start()
        {
            foreach (var element in _elements)
                element.StartDriven();

            base.Start();
        }

        public override void StartDriven()
        {
            foreach (var element in _elements)
                element.StartDriven();

            base.StartDriven();
        }

        protected override void PrepareRoutine()
        {
            var tasks = _elements.Where(e => !e.IsPrepared).Select(e => e.PrepareAsync()).ToArray();
            Task.WaitAll(tasks);
        }

        protected override void ImportRoutine()
        {
            var tasks = _elements.Where(e => !e.IsImported).Select(e => e.ImportAsync()).ToArray();
            Task.WaitAll(tasks);
        }

        protected override void PostProcessRoutine()
        {
            var tasks = _elements.Where(e => !e.IsPostProcessed).Select(e => e.PostProcessAsync()).ToArray();
            Task.WaitAll(tasks);
        }

        protected override void InterruptRoutine()
        {
            var tasks = _elements.Where(e => !e.IsInterrupted && !e.IsFinished).Select(e => e.InterruptAsync()).ToArray();
            Task.WaitAll(tasks);
        }

        protected override int GetTotalRecordsCount()
        {
            return _elements.Sum(e => e.TotalRecordsCount);
        }

        protected override int GetImportedRecordsCount()
        {
            return _elements.Sum(e => e.ImportedRecordsCount);
        }
    }
}
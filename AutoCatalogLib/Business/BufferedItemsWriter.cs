using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoCatalogLib.Business.CatalogImport;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Modules;
using AutoCatalogLib.Modules.Search;
using AutoCatalogLib.Persist;
using AutoCatalogLib.Persist.BusinessModels.Catalog;
using AutoCatalogLib.Utils;

namespace AutoCatalogLib.Business
{
    class BufferedItemsWriter : IBufferedItemsWriter
    {
        private static readonly ITextStammer Stammer = ModuleLocator.Resolve<ITextStammer>();
        private static readonly IPartItemFactory PartItemFactory = ModuleLocator.Resolve<IPartItemFactory>();

        private const int ContextItemsThreshold = 1000;     // items insert per one ef-context instance
        private const int ItemsBufferSize = 64;             // allow overrun items reader/writer
        private const int ItemsBufferIoTimeoutMsec = 100;   // thread blocking  when it cannot read/write buffer item

        private readonly ImportIdentificator _importIdentificator;
        private readonly ManagedCircleBuffer<CatalogImportItem> _buffer;

        private int _writenCount;
        public int WritenCount { get { return _writenCount; }}

        public BufferedItemsWriter(ImportIdentificator importIdentificator)
        {
            _importIdentificator = importIdentificator;
            _buffer = new ManagedCircleBuffer<CatalogImportItem>(ItemsBufferSize, ItemsBufferIoTimeoutMsec);
        }

        private IEnumerable<PartItem> ReadBuffer()
        {
            int itemsCount = 0;
            foreach (var item in _buffer.PopAsEnumerable())
            {
                _writenCount++;

                yield return BuildPartItem(item);
                if(itemsCount++ > ContextItemsThreshold)
                    yield break;
            }
        }

        private void ContextWriterTask()
        {
            while (!_buffer.Finished)
            {
                using (var context = new Context())
                {
                    context.MailformContext();
                    context.PartItems.AddRange(ReadBuffer());

                    if (!context.TrySaveChanges())
                        Log.Logger.Error(@"При загрузке данных не удалось обновить контекст");
                }
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            }
        }

        private PartItem BuildPartItem(CatalogImportItem importItem)
        {
            var item = PartItemFactory.BuildFrom(importItem);
            item.ImportIdentificatorId = _importIdentificator.Id;

            return item;
        }

        public void WriteItems(IEnumerable<CatalogImportItem> importItems)
        {
            var contextWriterTask = new Task(ContextWriterTask);
            contextWriterTask.Start();

            _buffer.PushRange(importItems);
            _buffer.Finish();

            contextWriterTask.Wait();
        }

        public Task WriteItemsAsync(IEnumerable<CatalogImportItem> importItems)
        {
            return Task.Factory.StartNew(() => WriteItems(importItems));
        }
    }
}

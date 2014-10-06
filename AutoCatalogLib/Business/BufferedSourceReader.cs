using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCatalogLib.Business.CatalogImport;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Modules;
using AutoCatalogLib.Utils;

namespace AutoCatalogLib.Business
{
    public class BufferedSourceReader : IBufferedSourceReader
    {
        private static readonly IRawRecordFormatterFactory RowFormatterFactory 
            = ModuleLocator.Resolve<IRawRecordFormatterFactory>();

        private const int BufferBlockingTimeout = 100;
        private const int BufferSize = 64;

        private readonly ISource _source;
        private readonly ITableRule _behavior;
        private IExternalReader _externalReader;

        private volatile ManagedCircleBuffer<CatalogImportItem> _buffer;

        private volatile bool _interrupted;

        private readonly IRawRecordRowFormatter _formatter;

        public int ReadedCount { get; private set; }
        public int TotalCount { get; private set; }

        public BufferedSourceReader(ISource source, ITableRule behavior)
        {
            if(behavior == null || source == null)
                throw new ArgumentNullException();

            _source = source;
            _behavior = behavior;

            _buffer = new ManagedCircleBuffer<CatalogImportItem>(BufferSize, BufferBlockingTimeout);
            _formatter = RowFormatterFactory.BuildTableRowFormatter(behavior);

            ReadedCount = -1;   // ToDo: убрать этот шозанах
            TotalCount = -1;
        }

        void BufferWriterRoutine()
        {
            ReadedCount = 0;
            try
            {
                foreach (var rawRow in _externalReader.GetRecords())
                {
                    _buffer.Push(CatalogItemSchema.BuildRecord(_formatter.Format(rawRow)));
                    ReadedCount++;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            finally
            {
                Close();
                _buffer.Finish();
            }
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        public void Interrupt()
        {
            _interrupted = true;
            _externalReader.Close();
        }

        public void Open()
        {
            if(_externalReader != null)
                throw new InvalidOperationException("Reader already opened");

            try
            {
                _externalReader = ReaderLocator.GetReader(_source, _behavior);
                _externalReader.Open();

                TotalCount = _externalReader.RecordsCount;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                Close();
                _buffer.Finish();
            }
        }

        public void Close()
        {
            if (_externalReader != null)
            {
                _externalReader.Close();
                _externalReader.Dispose();
                _externalReader = null;
            }
        }

        public IEnumerable<CatalogImportItem> ReadBuffer()
        {
            if (ReadedCount != -1)
                throw new Exception(@"Уже читается");

            Task.Factory.StartNew(BufferWriterRoutine);

            return _buffer.PopAsEnumerable();
        }

        public void Dispose()
        {
            Close();
        }
    }
}

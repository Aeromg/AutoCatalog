using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Exchange.ImportTargets;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Business.ImportUtils
{
    public class ImportTask : ImportTaskElement
    {
        private IBufferedSourceReader _sourceReader;
        private IBufferedItemsWriter _itemsWriter;
        private ISourceProfile _profile;
        private ImportIdentificator _importIdentificator;

        public ImportTask(ISourceProfile profile)
        {
            _profile = profile;
        }

        protected override void PrepareRoutine()
        {
            if (_profile.CleanBeforeImport)
                ImportToolkit.DropTransaction(_profile.TransactionIdentificator);

            _importIdentificator = ImportToolkit.CreateImportIdentificator(_profile);

            if (_profile.EmbedSource)
            {
                var blob = BlobToolkit.CreateFromFile(_profile.Source.File, _importIdentificator);
                _profile = new EmbeddedSourceProfile
                {
                    BlobName = blob.Name,
                    CleanBeforeImport = _profile.CleanBeforeImport,
                    EmbedSource = _profile.EmbedSource,
                    Guid = Guid.NewGuid(),
                    RuleIdentificatorString = _profile.Rule.Identificator,
                    TransactionIdentificator = _profile.TransactionIdentificator,
                    Distributor = _profile.Distributor
                };

                _importIdentificator = ImportToolkit.CreateImportIdentificator(_profile);
            }

            // ToDo: не только ITableRule!
            _sourceReader = new BufferedSourceReader(_profile.Source, _profile.Rule as ITableRule);
            _sourceReader.Open();

            _itemsWriter = new BufferedItemsWriter(_importIdentificator);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced); 
        }

        protected override void ImportRoutine()
        {
            _itemsWriter.WriteItems(_sourceReader.ReadBuffer());
        }

        protected override void PostProcessRoutine()
        {
            _sourceReader.Close();
            _sourceReader.Dispose();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        protected override void InterruptRoutine()
        {
            _sourceReader.Interrupt();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced); 
        }

        protected override int GetTotalRecordsCount()
        {
            return _sourceReader.TotalCount;
        }

        protected override int GetImportedRecordsCount()
        {
            return _itemsWriter.WritenCount;
        }
    }
}

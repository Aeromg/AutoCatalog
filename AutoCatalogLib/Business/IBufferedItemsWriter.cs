using System.Collections.Generic;
using System.Threading.Tasks;
using AutoCatalogLib.Business.CatalogImport;

namespace AutoCatalogLib.Business
{
    internal interface IBufferedItemsWriter
    {
        int WritenCount { get; }
        void WriteItems(IEnumerable<CatalogImportItem> items);
        Task WriteItemsAsync(IEnumerable<CatalogImportItem> items);
    }
}
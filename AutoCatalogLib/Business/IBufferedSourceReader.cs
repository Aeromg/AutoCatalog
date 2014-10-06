using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoCatalogLib.Business.CatalogImport;

namespace AutoCatalogLib.Business
{
    public interface IBufferedSourceReader : IDisposable
    {
        int ReadedCount { get; }
        int TotalCount { get; }
        void Open();
        void Close();
        IEnumerable<CatalogImportItem> ReadBuffer();
        void Interrupt();
    }
}
using System;
using System.Collections.Generic;

namespace AutoCatalogLib.Exchange
{
    public interface IExternalReader : IDisposable
    {
        int RecordsCount { get; }
        void Open();
        void Close();
        IEnumerable<RecordRow> GetRecords();
    }
}
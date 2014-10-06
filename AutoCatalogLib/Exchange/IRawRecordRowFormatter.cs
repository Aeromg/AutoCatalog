using System.Collections.Generic;

namespace AutoCatalogLib.Exchange
{
    public interface IRawRecordRowFormatter
    {
        RecordRow Format(RecordRow record);
        IEnumerable<RecordRow> Format(IEnumerable<RecordRow> record);
    }
}
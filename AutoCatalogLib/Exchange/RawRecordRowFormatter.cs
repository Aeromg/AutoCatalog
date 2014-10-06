using System.Collections.Generic;
using System.Linq;

namespace AutoCatalogLib.Exchange
{
    public class RawRecordRowFormatter : IRawRecordRowFormatter
    {
        private readonly IDictionary<string, IFormatter> _formatters;

        public RawRecordRowFormatter(IEnumerable<KeyValuePair<string, IFormatter>> formatters)
        {
            _formatters = formatters.ToDictionary(f => f.Key, f => f.Value);
        }

        public RecordRow Format(RecordRow record)
        {
            var formattedRecord = new RecordRow(record.Keys.Count)
            {
                Source = record.Source,
                SourceArgument = record.SourceArgument
            };

            foreach (var key in record.Keys)
                formattedRecord[key] = _formatters[key].Format(record[key]);

            return formattedRecord;
        }

        public IEnumerable<RecordRow> Format(IEnumerable<RecordRow> record)
        {
            return record.Select(Format);
        }
    }
}
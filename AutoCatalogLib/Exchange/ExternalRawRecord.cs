using System.Collections.Generic;

namespace AutoCatalogLib.Exchange
{
    /*
    class ExternalRawRecord
    {
        protected readonly IDictionary<string, object> Data;

        protected ExternalRawRecord()
        {
            Data = new Dictionary<string, object>();
        }

        public object this[string key]
        {
            get { return Data.ContainsKey(key) ? Data[key] : null; }
            set { Data[key] = value; }
        }

        public IEnumerable<string> Keys
        {
            get { return Data.Keys; }
        }

        public IEnumerable<object> Values
        {
            get { return Data.Values; }
        }

        object IExternalRawRecord.this[string key]
        {
            get { return Data.ContainsKey(key) ? Data[key] : null; }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }*/
}
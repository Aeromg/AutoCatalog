using System.Collections.Generic;

namespace AutoCatalogLib.Exchange
{
    /*
    class InternalRawRecord : ReaderRawRecord
    {
        private readonly IDictionary<string, object> _data;

        private InternalRawRecord()
        {
            _data = new Dictionary<string, object>();
        }

        public InternalRawRecord(IEnumerable<string> keys) : this()
        {
            foreach (var key in keys)
                _data[key] = null;
        }

        public IEnumerable<string> Properties
        {
            get { return _data.Keys; }
        }

        public IEnumerable<object> Values
        {
            get { return _data.Values; }
        }

        public object this[string property]
        {
            get { return _data.ContainsKey(property) ? _data[property] : null; }
            set { _data[property] = value; }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        } 
    }*/
}
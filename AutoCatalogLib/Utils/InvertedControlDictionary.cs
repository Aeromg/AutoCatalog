using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AutoCatalogLib.Utils
{
    internal class InvertedControlDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public Action<TKey, TValue> AddAction { get; set; }

        public Action<TKey, TValue> SetAction { get; set; }

        public Func<TKey, bool> ContainsKeyAction { get; set; }

        public Func<ICollection<TKey>> KeysGetter { get; set; }

        public Func<TKey, bool> RemoveAction { get; set; }

        public Func<TKey, TValue> GetValueAction { get; set; }

        public Func<ICollection<TValue>> ValuesGetter { get; set; }

        public Action ClearAction { get; set; }

        public Func<int> CountAction { get; set; }

        public Func<bool> IsReadonlyFunc { get; set; }

        public Func<IEnumerator<KeyValuePair<TKey, TValue>>> GetEnumeratorFunction { get; set; }

        //

        public void Add(TKey key, TValue value)
        {
            var action = AddAction;

            if (action != null)
                action(key, value);

            throw new Exception("AddAction not set");
        }

        public bool ContainsKey(TKey key)
        {
            var action = ContainsKeyAction;

            if (action != null)
                return action(key);

            TValue value;
            return TryGetValue(key, out value);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                var action = KeysGetter;
                return action != null ? action() : this.Select(i => i.Key).ToList();
            }
        }

        public bool Remove(TKey key)
        {
            var action = RemoveAction;

            return action != null && action(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var action = GetValueAction;

            if (action != null)
            {
                try
                {
                    value = action(key);
                    return true;
                }
                catch(Exception)
                {
                }
            }

            value = default(TValue);
            return false;
        }

        public ICollection<TValue> Values
        {
            get
            {
                var action = ValuesGetter;
                return action != null ? action() : this.Select(t => t.Value).ToList();
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (!TryGetValue(key, out value))
                    throw new KeyNotFoundException(key.ToString());

                return value;
            }
            set
            {
                if (!ContainsKey(key))
                    Add(key, value);

                Set(key, value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            var action = ClearAction;

            if (action != null)
                action();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key) && this[item.Key].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var keys = Keys.ToArray();
            for (int i = 0; i < keys.Length && i < array.Length; i++)
                array[i + arrayIndex] = new KeyValuePair<TKey, TValue>(keys[i], this[keys[i]]);
        }

        public int Count
        {
            get
            {
                var action = CountAction;
                return action != null ? action() : Keys.Count();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                var action = IsReadonlyFunc;
                return action != null ? action() : AddAction != null;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Contains(item) && Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var action = GetEnumeratorFunction;
            if (action != null)
                return action();

            throw new Exception("GetEnumeratorFunc not set");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Set(TKey key, TValue value)
        {
            var action = SetAction;
            if (action != null)
                action(key, value);

            throw new Exception("SetAction not set");
        }
    }
}
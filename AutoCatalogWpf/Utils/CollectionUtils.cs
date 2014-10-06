using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutoCatalogWpf.Utils
{
    public static class CollectionUtils
    {
        public static void UpdateObservableCollection<T>(ObservableCollection<T> collection, IEnumerable<T> items)
        {
            var toRemove = collection.Where(i => !items.Contains(i)).ToArray();
            var toAdd = items.Where(i => !collection.Contains(i)).ToArray();

            foreach (var item in toRemove)
                collection.Remove(item);

            foreach (var item in toAdd)
                collection.Add(item);
        }
    }
}
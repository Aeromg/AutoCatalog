using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoCatalogLib.Exchange;

namespace AutoCatalogLib.Business.CatalogImport
{
    public static class CatalogItemSchema
    {
        private static readonly IDictionary<string, PropertyInfo> PropertiesLookup = GetProperties();

        public static IEnumerable<string> Properties
        {
            get { return PropertiesLookup.Keys; }
        }

        public static Type GetPropertyConcreteType(string property)
        {
            return GetProperty(property).PropertyType;
        }

        public static object GetValue(CatalogImportItem record, string property)
        {
            return GetProperty(property).GetValue(record);
        }

        public static void SetValue(CatalogImportItem record, string property, object value)
        {
            GetProperty(property).SetValue(record, value);
        }

        private static PropertyInfo GetProperty(string property)
        {
            return PropertiesLookup[property];
        }

        private static IDictionary<string, PropertyInfo> GetProperties()
        {
            return typeof (CatalogImportItem).GetProperties().
                Where(p => p.GetCustomAttribute<CatalogImportItem.SpecialAttribute>() == null).
                ToDictionary(p => p.Name, p => p);
        }

        public static CatalogImportItem BuildRecord(RecordRow readerRawRecord)
        {
            var record = new CatalogImportItem();
            readerRawRecord.AsParallel().ForAll(kvp => record[kvp.Key] = kvp.Value);
            record.Source = readerRawRecord.Source;
            record.SourceArgument = readerRawRecord.SourceArgument;

            return record;
        }
    }
}

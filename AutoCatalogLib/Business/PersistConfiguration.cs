using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Persist;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Business
{
    public class PersistConfiguration
    {
        private static PersistConfiguration _instance;

        public static PersistConfiguration Instance
        {
            get { return _instance ?? (_instance = new PersistConfiguration()); }
        }

        public int SearchResultsLimit { get; set; }
        public bool UseSmartPrimarySearchEngine { get; set; }
        public bool UseRecursiveAnalogsSearchEngine { get; set; }
        public bool EnableViewItemSource { get; set; }
        public bool EnableGuiImport { get; set; }
        public string ProtectConfigPassword { get; set; }

        private PersistConfiguration()
        {
            SearchResultsLimit = 50;
            UseSmartPrimarySearchEngine = false;
            UseRecursiveAnalogsSearchEngine = false;
            EnableViewItemSource = false;
            EnableGuiImport = true;
            ProtectConfigPassword = "";
        }

        public void Load()
        {
            using (var context = new Context())
            {
                foreach (var prop in GetProperties())
                {
                    var value = GetPersistValue(prop.Name, context, prop.PropertyType);
                    if(value != null)
                        prop.SetValue(this, value);
                }
            }
        }

        public void Save()
        {
            using (var context = new Context())
            {
                foreach (var prop in GetProperties())
                    SetPersistValue(prop.Name, prop.GetValue(this), context);

                context.SaveChanges();
            }
        }

        private static object GetPersistValue(string name, Context context, Type valueType = null)
        {
            var element = context.ConfigElements.FirstOrDefault(e => e.Name == name);
            if (element == null)
                return GetDefault(valueType);

            var value = element.GetStoredValue();
            if (value == null)
                return GetDefault(valueType);

            return valueType != null && value.GetType() != valueType ? Convert.ChangeType(value, valueType) : value;
        }

        private static void SetPersistValue(string name, object value, Context context)
        {
            var element =
                context.ConfigElements.FirstOrDefault(e => e.Name == name) ?? new ConfigElement {Name = name};

            element.SetStoredValue(value);

            if(element.IsNew)
                context.ConfigElements.AddOrAttach(element);
        }

        private static void SavePersistValues(Context context)
        {
            context.SaveChanges();
        }

        private static IEnumerable<PropertyInfo> GetProperties()
        {
            var access = BindingFlags.Instance | BindingFlags.Public;

            return typeof(PersistConfiguration).GetProperties(access).Where(p => p.CanWrite);
        }

        public static object GetDefault(Type valueType)
        {
            if (valueType == null)
                return null;

            return valueType.IsValueType ? Activator.CreateInstance(valueType) : null;
        }
    }
}

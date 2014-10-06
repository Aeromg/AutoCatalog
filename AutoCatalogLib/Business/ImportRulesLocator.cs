using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Persist;
using AutoCatalogLib.Utils;

namespace AutoCatalogLib.Business
{
    public static class ImportRulesLocator
    {
        private static readonly IDictionary<string, IRule> Lookup;
        private static readonly ImportRulesPersist Persist = new ImportRulesPersist();

        private static bool _outdated = true;

        public static IRule GetBehavior(string identificator)
        {
            IRule behavior;
            if (Lookup.TryGetValue(identificator, out behavior))
                return behavior;

            behavior = GetBehaviorFromPersist(identificator);
            Lookup[identificator] = behavior;
            return behavior;
        }

        public static void SetBehavior(IRule behavior)
        {
            Lookup[behavior.Identificator] = behavior;
            Persist.SaveOrUpdate(behavior);
        }

        private static IRule GetBehaviorFromPersist(string identificator)
        {
            var behavior = Persist.SearchBehaviorByIdentificator(identificator);
            if (behavior != null) 
                return behavior;

            var ex = new Exception(@"Правило с идентификатором " + identificator + " не найдено");
            Log.Exception(ex);
            throw ex;
        }

        public static IRule SearchBehavior(string identificator)
        {
            IRule behavior;
            if (Lookup.TryGetValue(identificator, out behavior))
                return behavior;

            behavior = Persist.SearchBehaviorByIdentificator(identificator);

            return behavior;
        }

        static ImportRulesLocator()
        {
            Lookup = new Dictionary<string, IRule>();
        }

        public static void Remove(string identificator)
        {
            if (Lookup.ContainsKey(identificator))
                Lookup.Remove(identificator);

            Persist.Remove(identificator);
        }

        public static IEnumerable<IRule> GetBehaviors()
        {
            if (_outdated)
                UpdateLookup();

            return Lookup.Values;
        }

        private static void UpdateLookup()
        {
            var persisted = Persist.GetBehaviors();
            foreach (var persist in persisted.Where(persist => !Lookup.ContainsKey(persist.Identificator)))
                Lookup[persist.Identificator] = persist;

            _outdated = false;
        }
    }
}

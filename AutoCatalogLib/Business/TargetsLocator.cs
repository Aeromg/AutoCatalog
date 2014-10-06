using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Exchange.ImportTargets;

namespace AutoCatalogLib.Business
{
    public static class TargetsLocator
    {
        private static readonly IDictionary<Guid, ISourceProfile> Lookup = new Dictionary<Guid, ISourceProfile>();
        private static readonly TargetsPersist Persist = new TargetsPersist();
        private static bool _outdated = true;

        public static IEnumerable<ISourceProfile> Targets
        {
            get
            {
                if(_outdated)
                    UpdateLookup();

                return Lookup.Values;
            }
        }

        public static ISourceProfile GetTarget(Guid guid)
        {
            if(_outdated)
                UpdateLookup();

            return Lookup[guid];
        }

        public static void Set(ISourceProfile target)
        {
            CheckTarget(target);
            Lookup[target.Guid] = target;
            Persist.AddOrUpdate(target);
        }

        public static void Remove(ISourceProfile target)
        {
            CheckTarget(target);
            if(Lookup.ContainsKey(target.Guid))
                Lookup.Remove(target.Guid);

            Persist.Remove(target);
        }

        public static void Remove(Guid guid)
        {
            if (guid == Guid.Empty)
                throw new Exception(@"Указан пустой GUID");

            if (Lookup.ContainsKey(guid))
                Lookup.Remove(guid);

            Persist.Remove(guid);
        }

        private static void CheckTarget(ISourceProfile target)
        {
            if(target.Guid == Guid.Empty)
                throw new Exception(@"Target содержит пустой GUID");
        }

        private static void UpdateLookup()
        {
            var targets = Persist.GetAll().ToArray();

            foreach (var guid in Lookup.Keys.Where(l => !targets.Select(t => t.Guid).Contains(l)).ToArray())
                Lookup.Remove(guid);

            foreach (var target in targets.Where(t => !Lookup.Keys.Contains(t.Guid)))
                Lookup[target.Guid] = target;

            _outdated = false;
        }
    }
}

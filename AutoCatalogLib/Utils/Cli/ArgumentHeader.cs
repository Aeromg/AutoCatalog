using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoCatalogLib.Utils.Cli
{
    public class ArgumentHeader
    {
        public class ArgumentValue
        {
            public string Name { get; set; }
            public string Description { get; set; }

            private static readonly ArgumentValue _empty = new ArgumentValue { Name = @"empty", Description = @"" };
            public static ArgumentValue Empty
            {
                get { return _empty; }
            }
        }

        public string Token { get; set; }
        public string Description { get; set; }
        public bool IsSubArgument { get; set; }
        public ArgumentValue Value { get; set; }

        public bool HasSubArgument
        {
            get { return SubArguments.Any(); }
        }

        public bool HasDependencies
        {
            get { return Dependencies.Any(); }
        }

        public IEnumerable<ArgumentHeader> SubArguments
        {
            get { return Dependencies.Where(d => d.IsSubArgument); }
        }

        public IEnumerable<ArgumentHeader> Dependeds
        {
            get { return GetDependeds(this); }
        }

        public IEnumerable<ArgumentHeader> Dependencies
        {
            get { return GetDependencies(this); }
        }

        public IEnumerable<ArgumentHeader> ConflictsWith
        {
            get { return GetConflicts(this); }
        }

        public bool HasValue
        {
            get { return Value != ArgumentValue.Empty; }
        }

        public bool HasConflicts
        {
            get { return ConflictsWith.Any(); }
        }


        public static List<ArgumentHeader> Headers { get; private set; }

        private static IDictionary<ArgumentHeader, IList<ArgumentHeader>> _dependenciesDictionary 
            = new Dictionary<ArgumentHeader, IList<ArgumentHeader>>();

        private static IDictionary<ArgumentHeader, IList<ArgumentHeader>> _conflictsDictionary
            = new Dictionary<ArgumentHeader, IList<ArgumentHeader>>();

        public static void AddDependency(ArgumentHeader slave, ArgumentHeader master)
        {
            IList<ArgumentHeader> dependencies;
            if (!_dependenciesDictionary.TryGetValue(slave, out dependencies))
                _dependenciesDictionary[slave] = (dependencies = new List<ArgumentHeader>());

            dependencies.Add(master);
        }

        public static void AddConflict(ArgumentHeader master, ArgumentHeader slave)
        {
            IList<ArgumentHeader> conflicts;
            if (!_conflictsDictionary.TryGetValue(master, out conflicts))
                _conflictsDictionary[master] = (conflicts = new List<ArgumentHeader>());

            conflicts.Add(slave);
        }

        public static void AddConflict(ArgumentHeader master, params ArgumentHeader[] slaves)
        {
            foreach (var slave in slaves)
                AddConflict(master, slave);
        }

        public static void AddConflict(ArgumentHeader[] masters)
        {
            foreach (var master in masters)
                AddConflict(master, masters.Where(m => m != master).ToArray());
        }

        private static IEnumerable<ArgumentHeader> GetDependencies(ArgumentHeader slave)
        {
            IList<ArgumentHeader> dependencies;
            if (_dependenciesDictionary.TryGetValue(slave, out dependencies))
                return dependencies;

            return new ArgumentHeader[0];
        }

        private static IEnumerable<ArgumentHeader> GetDependeds(ArgumentHeader master)
        {
            return
                _dependenciesDictionary.Where(kvp => kvp.Value.Contains(master))
                    .Select(kvp => kvp.Key)
                    .Distinct();
        }

        public static IEnumerable<ArgumentHeader> GetConflicts(ArgumentHeader master)
        {
            IList<ArgumentHeader> conflicts;
            if (_conflictsDictionary.TryGetValue(master, out conflicts))
                return conflicts;

            return new ArgumentHeader[0];
        }

        public static ArgumentHeader GetHeader(string token)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            return Headers.First(h => h.Token == token);
        }

        public static bool TryGetHeader(string token, out ArgumentHeader header)
        {
            header = null;
            if (token == null)
                return false;

            header = Headers.FirstOrDefault(h => h.Token == token);
            return header != null;
        }

        static ArgumentHeader()
        {
            Headers = new List<ArgumentHeader>();
            ArgumentsDictionary.Register();
        }
    }
}
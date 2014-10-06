using System;
using System.Collections.Generic;
using System.Linq;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Exchange.ValueFormatters;
using AutoCatalogLib.Exchange.ValueFormatters.Impl;
using AutoCatalogLib.Utils;

namespace AutoCatalogLib.Business
{
    public static class FormattersLocator
    {
        private static readonly FormattersPersistService PersistService = new FormattersPersistService();

        private static readonly IDictionary<Guid, IFormatter> FormattersLookup = 
            new Dictionary<Guid, IFormatter>();

        private static bool _outdated = true;

        public static IEnumerable<IFormatter> Formatters
        {
            get { return FormattersLookup.Values; }
        }

        public static void Set(IFormatter formatter)
        {
            FormattersLookup[formatter.Guid] = formatter;
        }

        public static void Add(IFormatter formatter)
        {
            Set(formatter);
            PersistService.AddOrUpdate(formatter);
            _outdated = true;
        }

        public static void Remove(IFormatter formatter)
        {
            if (FormattersLookup.ContainsKey(formatter.Guid))
                FormattersLookup.Remove(formatter.Guid);

            PersistService.Remove(formatter);
            _outdated = true;
        }

        public static IFormatter Get(string guid)
        {
            return Get(Guid.Parse(guid));
        }

        public static IFormatter Get(Guid guid)
        {
            IFormatter formatter;

            if (FormattersLookup.TryGetValue(guid, out formatter))
                return formatter;

            formatter = PersistService.SearchFormatter(guid);
            FormattersLookup[guid] = formatter;

            if (formatter == null)
            {
                var ex = new Exception(@"Преобразователь " + guid + " не обнаружен");
                Log.Exception(ex);
                throw ex;
            }

            return formatter;
        }

        public static IFormatter<TReturn> Get<TReturn>(Guid guid)
        {
            return Get(guid) as IFormatter<TReturn>;
        }

        public static IFormatter Search(Guid guid)
        {
            IFormatter formatter;
            return FormattersLookup.TryGetValue(guid, out formatter) ? formatter : null;
        }

        public static IEnumerable<IFormatter> Search(Type destinationType)
        {
            return Formatters.Where(f => destinationType.IsAssignableFrom(f.Type));
        }

        public static IEnumerable<IFormatter<TReturn>> Search<TReturn>()
        {
            return Formatters.Where(f => typeof(TReturn).IsAssignableFrom(f.Type)).
                Cast<IFormatter<TReturn>>();
        }

        public static IEnumerable<IFormatter> GetFormatters()
        {
            if (_outdated)
                UpdateLookup();

            return Formatters;
        }

        private static void UpdateLookup()
        {
            var formatters = PersistService.EnumerateFormatters().ToArray();
            var keys = new HashSet<Guid>(formatters.Select(f => f.Guid));

            var dynamics = FormattersLookup.Where(kvp => kvp.Value is JavaScriptValueFormatter).Select(kvp => kvp.Key);
            foreach (var removed in dynamics.Where(k => !keys.Contains(k)).ToArray())
                FormattersLookup.Remove(removed);

            foreach (var formatter in formatters)
                Set(formatter);

            _outdated = false;
        }

        static FormattersLocator()
        {
            DefaultValueFormatters.Init();
        }
    }
}

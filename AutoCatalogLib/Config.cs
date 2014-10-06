using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Modules;
using AutoCatalogLib.Modules.Config;

namespace AutoCatalogLib
{
    public static class Config
    {
        private const string DbFileName = @"catalog.db";
        private static string _dbFilePathCached = @"";

        private static IConfigManager _manager;

        public static IConfigManager Manager
        {
            get { return _manager ?? (_manager = ModuleLocator.Resolve<IConfigManager>()); }
        }

        public static string GetDbPath()
        {
            return String.IsNullOrEmpty(_dbFilePathCached)
                ? _dbFilePathCached = ResolveDbPath()
                : _dbFilePathCached;
        }

        private static string ResolveDbPath()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            assemblyPath = Path.Combine(assemblyPath, DbFileName);

            var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            docsPath = Path.Combine(docsPath, DbFileName);

            var docsCatalogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"AutoCatalog");
            docsCatalogPath = Path.Combine(docsCatalogPath, DbFileName);

            var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"AutoCatalog");
            appDataPath = Path.Combine(appDataPath, DbFileName);

            if (File.Exists(assemblyPath))
                return assemblyPath;

            if (File.Exists(docsPath))
                return docsPath;

            if (File.Exists(docsCatalogPath))
                return docsCatalogPath;

            if (File.Exists(appDataPath))
                return appDataPath;

            throw new FileNotFoundException(DbFileName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using AutoCatalogLib.Business;
using AutoCatalogLib.Business.CatalogSearch;
using NetOffice.ExcelApi;

namespace AutoCatalogLib.Modules.Config
{
    [SingletonModule]
    public class ConfigManager : IConfigManager
    {
        public int SearchResultsLimit { get; set; }

        public bool UseSmartPrimarySearchEngine { get; set; }

        public bool UseRecursiveAnalogsSearchEngine { get; set; }

        public bool EnableViewItemSource { get; set; }

        public bool EnableGuiImport { get; set; }

        public string ProtectConfigPassword { get; set; }

        public ConfigManager()
        {
            /* SearchResultsLimit = 50;
            UseSmartPrimarySearchEngine = true; */
            LoadConfig();
        }

        public ISearch GetPrimarySearchEngine()
        {
            var engine = UseSmartPrimarySearchEngine
                ? ModuleLocator.Resolve<ISmartSearchEngine>() as ISearch
                : ModuleLocator.Resolve<IQuickSearchEngine>() as ISearch;

            engine.Limit = SearchResultsLimit;

            return engine;
        }

        public ISearch GetAnalogsSearchEngine()
        {
            var engine = ModuleLocator.Resolve<IAnalogSearchEngine>();
            engine.Limit = SearchResultsLimit;

            return engine;
        }

        public void LoadConfig()
        {
            var conf = PersistConfiguration.Instance;
            conf.Load();

            SearchResultsLimit = conf.SearchResultsLimit;
            UseSmartPrimarySearchEngine = conf.UseSmartPrimarySearchEngine;
            UseRecursiveAnalogsSearchEngine = conf.UseRecursiveAnalogsSearchEngine;
            EnableViewItemSource = conf.EnableViewItemSource;
            EnableGuiImport = conf.EnableGuiImport;
            ProtectConfigPassword = conf.ProtectConfigPassword;
        }

        public void SaveConfig()
        {
            var conf = PersistConfiguration.Instance;

            conf.SearchResultsLimit = SearchResultsLimit;
            conf.UseSmartPrimarySearchEngine = UseSmartPrimarySearchEngine;
            conf.UseRecursiveAnalogsSearchEngine = UseRecursiveAnalogsSearchEngine;
            conf.EnableViewItemSource = EnableViewItemSource;
            conf.EnableGuiImport = EnableGuiImport;
            conf.ProtectConfigPassword = ProtectConfigPassword;

            conf.Save();
        }
    }
}
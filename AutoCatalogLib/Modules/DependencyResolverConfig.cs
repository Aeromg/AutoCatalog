using System;
using System.Collections.Generic;
using System.Linq;
using AutoCatalogLib.Business.CatalogSearch;
using AutoCatalogLib.Modules.Config;
using Ninject.Modules;

namespace AutoCatalogLib.Modules
{
    internal sealed class DependencyResolverConfig : NinjectModule
    {
        public override void Load()
        {
            BindGeneralBehaviorModules();
            CustomConfig();
        }

        private void BindGeneralBehaviorModules()
        {
            var modulesSet = new HashSet<Type>();
            var modules = GetAssembliesModulesImplementations();
            foreach (var module in modules)
            {
                var moduleInterfaces = GetModuleInterfaceExtensions(module);
                foreach (var iface in moduleInterfaces)
                {
                    if (modulesSet.Contains(iface))
                        throw new Exception(@"Module already implemented");

                    var bindConfig = Bind(iface).To(module);

                    if (CheckIfSingletonModule(module))
                        bindConfig.InSingletonScope();

                    modulesSet.Add(iface);
                }
            }
        }

        private static bool CheckIfSingletonModule(Type module)
        {
            return module.GetCustomAttributes(typeof (SingletonModuleAttribute), true).Any();
        }

        private static IEnumerable<Type> GetModuleInterfaceExtensions(Type module)
        {
            return module.GetInterfaces().Where(i => typeof (IModule).IsAssignableFrom(i) && i != typeof (IModule));
        }

        private static IEnumerable<Type> GetAssembliesModulesImplementations()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly =>
                assembly.GetTypes().Where(t =>
                    typeof (IModule).IsAssignableFrom(t) &&
                    typeof (IModule) != t));
        }

        private void CustomConfig()
        {
        }
    }
}
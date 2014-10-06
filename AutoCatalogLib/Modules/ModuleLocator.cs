using System;
using Ninject;
using Ninject.Syntax;

namespace AutoCatalogLib.Modules
{
    public static class ModuleLocator
    {
        private static readonly IKernel NinjectKernel = new StandardKernel(new DependencyResolverConfig());

        public static T Resolve<T>()
        {
            return NinjectKernel.Get<T>();
        }

        public static T Resolve<T>(Type interfaceType)
        {
            return (T)NinjectKernel.Get(interfaceType);
        }

        public static IBindingToSyntax<TInterface> Bind<TInterface>()
        {
            return NinjectKernel.Bind<TInterface>();
        }
    }
}
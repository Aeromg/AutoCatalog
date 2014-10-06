using System.Reflection;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Modules;

namespace AutoCatalogLib.Business
{
    public static class ReaderLocator
    {
        public static IExternalReader GetReader(ISource source, IRule behavior)
        {
            var behaviorType = behavior.GetType();
            var sourceAttrib = behaviorType.GetCustomAttribute<SourcePointerAttribute>(true);
            var readerFactory = ModuleLocator.Resolve<IExternalReaderFactory>(sourceAttrib.ReaderFactory);
            return readerFactory.GetReader(source, behavior);
        }
    }
}

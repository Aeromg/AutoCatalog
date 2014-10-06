using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Business.Invokers;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Modules;

namespace AutoCatalogLib.Business
{
    public static class ExternalInvokerLocator
    {
        public static IExternalInvokerFactory GetFactory(ISource source)
        {
            return GetFactory(source.Location);
        }

        public static IExternalInvokerFactory GetFactory(string location)
        {
            // ToDo: нормальное IoC вместо этого быдлокода
            if (location.EndsWith(@".xls") || location.EndsWith(@".xlsx"))
            {
                return ModuleLocator.Resolve<IExcelInvokerFactory>();
            }

            throw new NotImplementedException("Location " + location + " does not supported yet.");
        }
    }
}

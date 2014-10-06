using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Exchange;

namespace AutoCatalogLib.Business
{
    public static class SourceInvoker
    {
        public static void Invoke(ISource source, string arguments)
        {
            var invoker = ExternalInvokerLocator.GetFactory(source).GetInvoker();
            invoker.Invoke(source, arguments);
        }

        public static bool IsInvokable(ISource source, string arguments)
        {
            var invoker = ExternalInvokerLocator.GetFactory(source).GetInvoker();
            return invoker.IsInvokable(source, arguments);
        }
    }
}

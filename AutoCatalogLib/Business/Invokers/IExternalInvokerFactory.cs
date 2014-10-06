using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Modules;

namespace AutoCatalogLib.Business.Invokers
{
    public interface IExternalInvokerFactory
    {
        IExternalSourceInvoker GetInvoker();
    }
}

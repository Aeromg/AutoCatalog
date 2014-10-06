using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Modules;

namespace AutoCatalogLib.Business.Invokers
{
    [SingletonModule]
    public class ExcelInvokerFactory : IExcelInvokerFactory
    {
        private static volatile ExcelSourceInvoker _invoker;

        public IExternalSourceInvoker GetInvoker()
        {
            return GetSingleton();
        }

        private ExcelSourceInvoker GetSingleton()
        {
            if (_invoker != null)
                return _invoker;

            _invoker = new ExcelSourceInvoker();
            _invoker.Closed += InvokerOnClosed;

            return _invoker;
        }

        private void InvokerOnClosed(object sender, EventArgs eventArgs)
        {
            _invoker.Dispose();
            _invoker = null;
        }
    }
}

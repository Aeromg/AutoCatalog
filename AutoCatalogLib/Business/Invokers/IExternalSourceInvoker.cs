using System;
using AutoCatalogLib.Exchange;

namespace AutoCatalogLib.Business.Invokers
{
    public interface IExternalSourceInvoker : IDisposable
    {
        void Invoke(ISource source, string arguments);
        bool IsInvokable(ISource source, string arguments);
        event EventHandler Closed;
    }
}

using System;
using System.Collections.Generic;
using AutoCatalogLib.Modules;

namespace AutoCatalogLib.JavaScript
{
    public interface IJavaScriptEnvironment : IDisposable, IModule
    {
        IDictionary<string, object> DefaultParameters { get; }
        string Script { get; set; }
        object this[string parameter] { get; set; }
        void Reset();
        void Run();
    }
}
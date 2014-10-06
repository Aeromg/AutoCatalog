using System;

namespace AutoCatalogLib.Exchange
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class SourcePointerAttribute : Attribute
    {
        public string SourceMember { get; set; }
        public Type ReaderFactory { get; set; }
    }
}

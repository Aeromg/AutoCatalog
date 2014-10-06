using System;
using AutoCatalogLib.Exchange;

namespace AutoCatalogLib.Business.CatalogImport
{
    public class CatalogImportItem
    {
        public class SpecialAttribute : Attribute
        {
        }

        public string Name { get; set; }
        public string Brand { get; set; }
        public string PartNumber { get; set; }
        public double Price { get; set; }
        public double Balance { get; set; }
        public string[] Analogs { get; set; }
        public string Distributor { get; set; }
        public string Commentary { get; set; }
        
        [Special]
        public ISource Source { get; set; }

        [Special]
        public string SourceArgument { get; set; }

        public object this[string property]
        {
            get { return CatalogItemSchema.GetValue(this, property); }
            set { CatalogItemSchema.SetValue(this, property, value); }
        }
    }
}

using System.ComponentModel.DataAnnotations;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Persist.BusinessModels.Catalog
{
    public class Currency : Entity
    {
        [StringLength(256)]
        [Display(Name = @"Currency name")]
        public string Name { get; set; }

        [Display(Name = @"Conversion rate")]
        public double ConversionRate { get; set; }
    }
}
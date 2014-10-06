using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Persist.BusinessModels.Catalog
{
    public class PartItem : Entity
    {
        [NotMapped]
        private string [] _analogsPartNumbers;

        [Display(Name = @"Displayed name")]
        public string Name { get; set; }

        [Display(Name = @"Brand part number")]
        public string PartNumber { get; set; }

        [Display(Name = @"Distributor commentary")]
        public string Commentary { get; set; }

        [Display(Name = @"Part price")]
        public double Price { get; set; }

        [Display(Name = @"Distributor balance")]
        public double Balance { get; set; }

        [Display(Name = @"SourceArgument")]
        public string SourceArgument { get; set; }

        [Display(Name = @"Brand")]
        public string Brand { get; set; }

        [Display(Name = @"Part analogs")]
        public string Analogs { get; set; }

        [NotMapped]
        public string[] AnalogsPartNumbers
        {
            get
            {
                if (_analogsPartNumbers != null)
                    return _analogsPartNumbers;

                return (_analogsPartNumbers = String.IsNullOrEmpty(Analogs) ? new string[0] : Analogs.Split(' '));
            }
        }

        public long ImportIdentificatorId { get; set; }

        public string StammedPartNumber { get; set; }

        public string SearchString { get; set; }

    }
}
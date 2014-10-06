using System;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Persist.BusinessModels.ExcelFormat
{
    public class ExcelImportRuleCellPoco : Entity
    {
        public string Property { get; set; }
        public int RowIndex { get; set; }
        public Guid FormatterGuid { get; set; }
        public long ExcelImportRulePocoId { get; set; }
        public bool Active { get; set; }
        public bool Required { get; set; }
    }
}
using System.Linq;
using AutoCatalogLib.Business;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Exchange.ExcelFormat;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Persist.BusinessModels.ExcelFormat
{
    class ExcelFileRuleBridge : Bridge<ExcelImportRule, ExcelImportRulePoco>
    {
        public ExcelFileRuleBridge(ExcelImportRulePoco entity) : base(entity) { }

        protected override ExcelImportRule GetModel(ExcelImportRulePoco entity)
        {
            return new ExcelImportRule()
            {
                Name = entity.Name,
                Description = entity.Description,
                Identificator = entity.Identificator,
                CsvMode = entity.UseCsvFile,
                RowOffset = entity.RowOffset,
                ColumnOffset = entity.ColumnOffset,
                WorksheetIndex = entity.WorksheetIndex,
                Columns = entity.Properties.Select(p => new ExcelImportRule.Column()
                {
                    Index = p.RowIndex,
                    Name = p.Property.ToString(),
                    Formatter = FormattersLocator.Get(p.FormatterGuid),
                    Active = p.Active,
                    Required = p.Required
                })
            };
        }

        protected override void UpdateEntity(ExcelImportRulePoco entity, ExcelImportRule model)
        {
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.UseCsvFile = model.CsvMode;
            entity.Identificator = model.Identificator;
            entity.RowOffset = model.RowOffset;
            entity.ColumnOffset = model.ColumnOffset;
            entity.WorksheetIndex = model.WorksheetIndex;
            
            foreach (var column in model.Columns)
            {
                var prop = entity.Properties.First(p => p.Property == column.Name);

                prop.RowIndex = column.Index;
                prop.FormatterGuid = column.Formatter.Guid;
                prop.Active = column.Active;
                prop.Required = column.Required;
            }
        }
    }
}

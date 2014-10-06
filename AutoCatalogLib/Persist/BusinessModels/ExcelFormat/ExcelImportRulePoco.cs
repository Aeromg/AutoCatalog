using System;
using System.Collections.Generic;
using System.Linq;
using AutoCatalogLib.Business;
using AutoCatalogLib.Business.CatalogImport;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Exchange.ExcelFormat;
using AutoCatalogLib.Persist.Generic;
using AutoCatalogLib.Utils;

namespace AutoCatalogLib.Persist.BusinessModels.ExcelFormat
{
    public class ExcelImportRulePoco : Entity, IBridgeable<ExcelImportRule, ExcelImportRulePoco>
    {
        private Bridge<ExcelImportRule, ExcelImportRulePoco> _bridge;
        private IEnumerable<ExcelImportRuleCellPoco> _properties;

        public string Name { get; set; }
        public string Description { get; set; }
        public string Identificator { get; set; }
        public int RowOffset { get; set; }
        public int ColumnOffset { get; set; }
        public int WorksheetIndex { get; set; }
        public bool UseCsvFile { get; set; }

        public IEnumerable<ExcelImportRuleCellPoco> Properties
        {
            get { return _properties ?? (_properties = BuildProperties()); }
        }

        internal override void BeforeStateChanged(Context context, EntityChangeState state)
        {
            // все поля должны ссылаться на меня
            if (state == EntityChangeState.Modify)
            {
                if (Properties.Any(p => p.ExcelImportRulePocoId != Id))
                {
                    var ex = Log.Exception(new Exception(@"Обновляемое правило импорта содержит недопустимые поля"));
#if DEBUG
                    throw ex;
#endif
                }
            }

            // записываем поля
            if (state == EntityChangeState.Add)
            {
                foreach (var prop in Properties)
                {
                    if (!prop.IsNew)
                    {
                        var ex = new Exception(@"Правило импорта записывается раньше собственных полей");
                        Log.Exception(ex);
#if DEBUG
                        throw ex;
#endif
                    }
                }
            }
        }

        internal override void AfterStateChanged(Context context, EntityChangeState state)
        {
            // обновить поля
            if (state == EntityChangeState.Add)
            {
                foreach (var prop in Properties)
                {
                    prop.ExcelImportRulePocoId = Id;
                    context.ExcelImportRuleCell.AddOrAttach(prop);
                }

                context.SaveChanges();
            }

            // восстановить поля из базы
            if (state == EntityChangeState.Loaded)
            {
                _properties = context.ExcelImportRuleCell.Where(c => c.ExcelImportRulePocoId == Id).ToArray();
#if DEBUG
                if(_properties.Count() != CatalogItemSchema.Properties.Count())
                    throw new Exception(@"В правиле импорта не хватает полей");
#endif
            }

            // прибить поля тоже
            if (state == EntityChangeState.Delete)
            {
                if (Properties == null || Properties.Any(p => p.ExcelImportRulePocoId != Id))
                {
                    var ex = new Exception(@"Удаляемое правило импорта содержит некорректные поля");
                    Log.Exception(ex);
#if DEBUG
                    throw ex;
#endif
                }

                context.ExcelImportRuleCell.Remove(Properties);
                context.SaveChanges();
            }
        }

        private IEnumerable<ExcelImportRuleCellPoco> BuildProperties()
        {
            if (!IsNew)
            {
                var ex = new Exception(@"Попытка создания пустых полей для записанного правила импорта");
                Log.Exception(ex);
#if DEBUG
                throw ex;
#endif
            }

            return (
                from property in CatalogItemSchema.Properties
                select new ExcelImportRuleCellPoco()
                {
                    RowIndex = 0,
                    Property = property,
                    FormatterGuid = FormattersLocator.Search(CatalogItemSchema.GetPropertyConcreteType(property)).First().Guid
                }).ToArray();
        }

        public Bridge<ExcelImportRule, ExcelImportRulePoco> Bridge
        {
            get { return _bridge ?? (_bridge = new ExcelFileRuleBridge(this)); }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Exchange.ExcelFormat;
using AutoCatalogLib.Persist;
using AutoCatalogLib.Persist.BusinessModels.ExcelFormat;

namespace AutoCatalogLib.Business
{
    class ImportRulesPersist
    {
        private readonly ContextHolder _holder = new ContextHolder();

        public IRule SearchBehaviorByIdentificator(string identificator)
        {
            return SearchExcelBehaviorByIdentificator(identificator);
        }

        private ExcelImportRule SearchExcelBehaviorByIdentificator(string identificator)
        {
            var entity = _holder.Context.ExcelImportRule.FirstOrDefault(e => e.Identificator == identificator);

            return entity == null ? null : entity.Bridge.GetModel();
        }

        public void SaveOrUpdate(IRule behavior)
        {
            if (behavior is ExcelImportRule)
            {
                SaveOrUpdateExcel(behavior as ExcelImportRule);
                return;
            }

            throw new NotImplementedException();
        }

        private void SaveOrUpdateExcel(ExcelImportRule behavior)
        {
            var context = _holder.Context;

            var entity =
                context.ExcelImportRule.FirstOrDefault(e => e.Identificator == behavior.Identificator) ??
                new ExcelImportRulePoco();

            entity.Bridge.FromModel(behavior);
            if (entity.IsNew)
                entity.AddOrAttach(context);

            context.SaveChanges();
        }

        public IEnumerable<IRule> GetBehaviors()
        {
            return _holder.Context.ExcelImportRule.ToArray().Select(e => e.Bridge.GetModel());
        }

        internal void Remove(string identificator)
        {
            var entity = _holder.Context.ExcelImportRule.First(e => e.Identificator == identificator);
            _holder.Context.ExcelImportRule.Remove(entity);
            _holder.Context.SaveChanges();
        }
    }
}

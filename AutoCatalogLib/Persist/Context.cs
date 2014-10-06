using System;
using AutoCatalogLib.Persist.BusinessModels.Catalog;
using AutoCatalogLib.Persist.BusinessModels.Dynamic.Formatters;
using AutoCatalogLib.Persist.BusinessModels.ExcelFormat;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Persist
{
    public class Context : GenericContext
    {
        private static readonly ContextHolder Holder = new ContextHolder();

        public Context() : base("catalog")
        {
            BeforeSetChanged += (sender, args) => args.Entity.BeforeStateChanged(this, args.State);
            AfterSetChanged += (sender, args) => args.Entity.AfterStateChanged(this, args.State);
        }

        public static Context Default
        {
            get { return Holder.Context; }
        }

        // see base class
        public GenericRepository<PartItem> PartItems { get; private set; }
        /* public GenericRepository<Distributor> Distributors { get; private set; }
        public GenericRepository<Currency> Currencies { get; private set; } */
        public GenericRepository<EmbeddedBlob> EmbeddedBlobs { get; private set; }
        public GenericRepository<CustomFormatterReference> ScriptedValueFormatters { get; private set; }
        public GenericRepository<FixedFormatterReference> FixedValueFormatters { get; private set; }
        public GenericRepository<ExcelImportRuleCellPoco> ExcelImportRuleCell { get; private set; }
        public GenericRepository<ExcelImportRulePoco> ExcelImportRule { get; private set; }
        public GenericRepository<ImportTargetPoco> ImportTargets { get; private set; }
        public GenericRepository<ImportIdentificator> ImportIdentificators { get; private set; }
        public GenericRepository<ConfigElement> ConfigElements { get; private set; }

        public static new void Drop()
        {
            GenericContext.Drop();
        }

        public static new void Vacuum()
        {
            GenericContext.Vacuum();
        }

        public static new void Sql(string sql)
        {
            GenericContext.Sql(sql);
        }

        public static void CloseAll()
        {
            GenericContext.CloseAll();
        }
    }
}
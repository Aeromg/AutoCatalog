using System;
using System.Collections.Generic;
using System.Linq;
using AutoCatalogLib.Business.CatalogImport;
using AutoCatalogLib.Business.ImportUtils;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Exchange.ImportTargets;
using AutoCatalogLib.Modules;
using AutoCatalogLib.Modules.Search;
using AutoCatalogLib.Persist;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Business
{
    public static class ImportToolkit
    {
        private static readonly ITextStammer Stammer = ModuleLocator.Resolve<ITextStammer>();

        public static IImportTask CreateImportTask(ISourceProfile target)
        {
            return new ImportTask(target);
        }

        public static IImportTask CreateImportTask(IEnumerable<ISourceProfile> targets)
        {
            return new ImportTaskMultiElement(targets.Select(t => new ImportTask(t)));
        }

        public static ImportIdentificator CreateImportIdentificator(ISourceProfile target)
        {
            using (var context = new Context())
            {
                var idName = target.TransactionIdentificator + "_";
                idName += context.ImportIdentificators.Count(i => i.Identificator.StartsWith(idName));
                var id = new ImportIdentificator
                {
                    DateTime = DateTime.Now,
                    Identificator = idName,
                    SourceLocation = target.Source.Location,
                    Distributor = target.Distributor
                };
                
                context.ImportIdentificators.AddOrAttach(id);
                context.SaveChanges();

                return id;
            }
        }

        public static void DropTransaction(string transactionId)
        {
            var transactionPrefix = transactionId + "_";
            var context = Context.Default;
            var transactions = context.ImportIdentificators.Where(t => t.Identificator.StartsWith(transactionPrefix)).ToArray();
            var transactionIds = transactions.Select(t => t.Id).ToArray();

            Context.Sql("DELETE from EmbeddedBlobs WHERE ImportIdentificatorId IN (" + String.Join(",", transactionIds) + ");");
            Context.Sql("DELETE from PartItems WHERE ImportIdentificatorId IN (" + String.Join(",", transactionIds) + ");");
            //context.PartItems.Remove(context.PartItems.Where(p => transactionIds.Contains(p.ImportIdentificatorId)));
            //context.ImportIdentificators.Remove(transactions);
            Context.Sql("DELETE from ImportIdentificators WHERE Id IN (" + String.Join(",", transactionIds) + ");");
            //context.SaveChanges();
            //context.Dispose();

            Context.Vacuum();
        }

        public static void DropTransaction()
        {
            /*var context = Context.Default;
            context.PartItems.Remove(context.PartItems);
            context.ImportIdentificators.Remove(context.ImportIdentificators);
            context.SaveChanges();
            context.Dispose();*/

            Context.Sql("DELETE from EmbeddedBlobs;");
            Context.Sql("DELETE from PartItems;");
            Context.Sql("DELETE from ImportIdentificators;");

            Context.Vacuum();
        }

        public static ImportIdentificator SearchImportInfo(PartItem partItem)
        {
            using (var context = new Context())
            {
                return context.ImportIdentificators.FirstOrDefault(i => i.Id == partItem.ImportIdentificatorId);
            }
        }
    }
}

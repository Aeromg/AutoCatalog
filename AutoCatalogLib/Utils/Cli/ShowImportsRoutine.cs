using System;
using System.Linq;
using AutoCatalogLib.Persist;

namespace AutoCatalogLib.Utils.Cli
{
    class ShowImportsRoutine : Routine
    {
        public override void Run()
        {
            var transactions = Context.Default.ImportIdentificators.Select(t => t.Identificator).ToArray();

            transactions = transactions.Select(t =>
            {
                var splitIndex = t.IndexOf('_');
                return (splitIndex > 0)
                    ? t.Substring(0, splitIndex)
                    : t;
            }).Distinct().ToArray();

            Console.WriteLine(@"����� ������ ��������");
            Console.WriteLine();
            Console.WriteLine(@"��������:");
            foreach (var transaction in transactions)
            {
                Console.WriteLine("-\t" + transaction);
            }
            Console.WriteLine();

            Console.WriteLine(@"��������:");
            Console.WriteLine();
            foreach (var transaction in Context.Default.ImportIdentificators)
            {
                Console.WriteLine(@"������ {0} �� {1}", transaction.Identificator, transaction.DateTime);
                Console.WriteLine(@"��������: " + transaction.SourceLocation);
                Console.WriteLine(@"�������: " + Context.Default.PartItems.Count(p => p.ImportIdentificatorId == transaction.Id));
                Console.WriteLine();
            }

            Console.WriteLine(@"���������.");
        }
    }
}
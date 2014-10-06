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

            Console.WriteLine(@"Вывод списка импортов");
            Console.WriteLine();
            Console.WriteLine(@"Основные:");
            foreach (var transaction in transactions)
            {
                Console.WriteLine("-\t" + transaction);
            }
            Console.WriteLine();

            Console.WriteLine(@"Подробно:");
            Console.WriteLine();
            foreach (var transaction in Context.Default.ImportIdentificators)
            {
                Console.WriteLine(@"Импорт {0} от {1}", transaction.Identificator, transaction.DateTime);
                Console.WriteLine(@"Источник: " + transaction.SourceLocation);
                Console.WriteLine(@"Записей: " + Context.Default.PartItems.Count(p => p.ImportIdentificatorId == transaction.Id));
                Console.WriteLine();
            }

            Console.WriteLine(@"Завершено.");
        }
    }
}
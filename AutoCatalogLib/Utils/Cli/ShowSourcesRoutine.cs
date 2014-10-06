using System;
using System.Linq;
using AutoCatalogLib.Business;
using AutoCatalogLib.Persist;

namespace AutoCatalogLib.Utils.Cli
{
    class ShowSourcesRoutine : Routine
    {
        public override void Run()
        {
            var rules = Context.Default.ExcelImportRule.Select(r => r.Identificator).ToArray();

            Console.WriteLine(@"Вывод списка источников");
            Console.WriteLine(@"Основные:");
            foreach (var target in TargetsLocator.Targets)
            {
                Console.WriteLine("-\t" + target.TransactionIdentificator);
            }

            Console.WriteLine();

            Console.WriteLine(@"Подробно:");
            Console.WriteLine();
            foreach (var target in TargetsLocator.Targets)
            {
                Console.WriteLine(@"Источник: " + target.Source.Location);
                Console.WriteLine(@"Правило: " + target.Rule.Identificator + " (" + target.Rule.Name + ")");
                Console.WriteLine(@"Идентификатор: " + target.TransactionIdentificator);
                Console.WriteLine(@"GUID: " + target.Guid);
                Console.WriteLine();
            }

            Console.WriteLine(@"Завершено.");
        }
    }
}
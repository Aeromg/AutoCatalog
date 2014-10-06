using System;
using System.Linq;
using AutoCatalogLib.Persist;

namespace AutoCatalogLib.Utils.Cli
{
    class ShowRulesRoutine : Routine
    {
        public override void Run()
        {
            var rules = Context.Default.ExcelImportRule.Select(r => r.Identificator).ToArray();

            Console.WriteLine(@"Вывод списка правил импорта");
            Console.WriteLine(@"Основные:");
            foreach (var rule in rules)
            {
                Console.WriteLine("-\t" + rule);
            }
            Console.WriteLine();

            Console.WriteLine(@"Подробно:");
            Console.WriteLine();
            foreach (var rule in Context.Default.ExcelImportRule)
            {
                Console.WriteLine(@"Название: " + rule.Name);
                Console.WriteLine(@"Описание: " + rule.Description);
                Console.WriteLine(@"Идентификатор: " + rule.Identificator);
                Console.WriteLine();
            }

            Console.WriteLine(@"Завершено.");
        }
    }
}
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

            Console.WriteLine(@"����� ������ ������ �������");
            Console.WriteLine(@"��������:");
            foreach (var rule in rules)
            {
                Console.WriteLine("-\t" + rule);
            }
            Console.WriteLine();

            Console.WriteLine(@"��������:");
            Console.WriteLine();
            foreach (var rule in Context.Default.ExcelImportRule)
            {
                Console.WriteLine(@"��������: " + rule.Name);
                Console.WriteLine(@"��������: " + rule.Description);
                Console.WriteLine(@"�������������: " + rule.Identificator);
                Console.WriteLine();
            }

            Console.WriteLine(@"���������.");
        }
    }
}
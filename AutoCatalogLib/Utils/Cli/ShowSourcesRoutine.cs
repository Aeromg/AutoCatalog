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

            Console.WriteLine(@"����� ������ ����������");
            Console.WriteLine(@"��������:");
            foreach (var target in TargetsLocator.Targets)
            {
                Console.WriteLine("-\t" + target.TransactionIdentificator);
            }

            Console.WriteLine();

            Console.WriteLine(@"��������:");
            Console.WriteLine();
            foreach (var target in TargetsLocator.Targets)
            {
                Console.WriteLine(@"��������: " + target.Source.Location);
                Console.WriteLine(@"�������: " + target.Rule.Identificator + " (" + target.Rule.Name + ")");
                Console.WriteLine(@"�������������: " + target.TransactionIdentificator);
                Console.WriteLine(@"GUID: " + target.Guid);
                Console.WriteLine();
            }

            Console.WriteLine(@"���������.");
        }
    }
}
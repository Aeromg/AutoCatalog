using System;
using AutoCatalogLib.Business;
using AutoCatalogLib.Business.Invokers;
using AutoCatalogLib.Persist.BusinessModels.Catalog;
using AutoCatalogLib.Utils.Cli;

namespace AutoCatalogLib
{
    public class AutoCatalog
    {
        public static void Main(string[] args)
        {
            /*
            Context.Drop();
            TestUtils.DefaultRules();

            var proxy = ImportToolkit.Import(TargetsLocator.Targets.First());
            proxy.Start();
            proxy.ProgressChanged += (sender, eventArgs) => Console.WriteLine(proxy.ProgressCount);
            Console.ReadKey();
            return;  

            //var proxy = ImportToolkit.Import(TargetsLocator.Targets.First());
            //proxy.Start();

            var a = Context.Default.PartItems.SqlAppend(@"SearchString LIKE '%насос%'").ToArray();

            Console.ReadKey();

            return; */

            TestUtils.SimpleStupidLocker();
            //TestUtils.DefaultRules();
            var cli = new CliApplication(String.Join(" ", args));
            //var cli = new CliApplication(@"--export-db c:\Temp\db.gz");
            cli.Main(); 
        }
    }
}
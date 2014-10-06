using System;
using System.IO;

namespace AutoCatalogLib.Utils.Cli
{
    class BatchRoutine : Routine
    {
        public override void Run()
        {
            var batchfile = Arguments.First(ArgumentsDictionary.Batch).Value;
            Console.WriteLine(@"���������� ��������� ����� " + batchfile);
            using (var input = File.OpenText(batchfile))
            {
                while (!input.EndOfStream)
                {
                    new CliApplication(input.ReadLine()).Main();
                }
            }
            Console.WriteLine(@"���� " + batchfile + @" ��������");
        }
    }
}
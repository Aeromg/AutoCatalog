using System;
using AutoCatalogLib.Business;

namespace AutoCatalogLib.Utils.Cli
{
    class DropRecordsRoutine : Routine
    {
        public override void Run()
        {
            Console.WriteLine(@"�������� �������");

            if (!Arguments.Has(ArgumentsDictionary.IKnowWhatImDoing))
            {
                if (!RequestAck(@"������ ����� �������."))
                {
                    Console.WriteLine(@"�������� �������������");
                    return;
                }
            }
            ImportToolkit.DropTransaction();
            Console.WriteLine(@"������ �������.");
        }
    }
}
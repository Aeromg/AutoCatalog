using System;
using AutoCatalogLib.Business;

namespace AutoCatalogLib.Utils.Cli
{
    class DropImportRoutine : Routine
    {
        public override void Run()
        {
            var id = Arguments.First(ArgumentsDictionary.DropImport).Value;
            if (String.IsNullOrEmpty(id))
                Console.WriteLine("����������� ������������� �������");

            Console.WriteLine("�������� ������� � ��������������� " + id);

            if (!Arguments.Has(ArgumentsDictionary.IKnowWhatImDoing))
            {
                if (!RequestAck(@"������ ����� �������."))
                {
                    Console.WriteLine(@"�������� �������������");
                    return;
                }
            }
            ImportToolkit.DropTransaction(id);
            Console.WriteLine(@"������ �������.");
        }
    }
}
using System;
using AutoCatalogLib.Persist;

namespace AutoCatalogLib.Utils.Cli
{
    class DropEverythingRoutine : Routine
    {
        public override void Run()
        {
            Console.WriteLine("������ ������� ���� ������");
            if (!Arguments.Has(ArgumentsDictionary.IKnowWhatImDoing))
            {
                if (!RequestAck(@"������ ����� �������."))
                {
                    Console.WriteLine(@"�������� �������������");
                    return;
                }
            }

            Context.Drop();
            Console.WriteLine(@"������ �������.");
        }
    }
}
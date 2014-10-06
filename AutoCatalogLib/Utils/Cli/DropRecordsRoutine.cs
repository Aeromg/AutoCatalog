using System;
using AutoCatalogLib.Business;

namespace AutoCatalogLib.Utils.Cli
{
    class DropRecordsRoutine : Routine
    {
        public override void Run()
        {
            Console.WriteLine(@"Удаление записей");

            if (!Arguments.Has(ArgumentsDictionary.IKnowWhatImDoing))
            {
                if (!RequestAck(@"Данные будут удалены."))
                {
                    Console.WriteLine(@"Отменено пользователем");
                    return;
                }
            }
            ImportToolkit.DropTransaction();
            Console.WriteLine(@"Данные удалены.");
        }
    }
}
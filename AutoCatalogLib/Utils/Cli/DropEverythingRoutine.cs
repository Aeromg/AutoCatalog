using System;
using AutoCatalogLib.Persist;

namespace AutoCatalogLib.Utils.Cli
{
    class DropEverythingRoutine : Routine
    {
        public override void Run()
        {
            Console.WriteLine("Полная очистка базы данных");
            if (!Arguments.Has(ArgumentsDictionary.IKnowWhatImDoing))
            {
                if (!RequestAck(@"Данные будут удалены."))
                {
                    Console.WriteLine(@"Отменено пользователем");
                    return;
                }
            }

            Context.Drop();
            Console.WriteLine(@"Данные удалены.");
        }
    }
}
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
                Console.WriteLine("Отсутствует идентификатор импорта");

            Console.WriteLine("Удаление импорта с идентификатором " + id);

            if (!Arguments.Has(ArgumentsDictionary.IKnowWhatImDoing))
            {
                if (!RequestAck(@"Данные будут удалены."))
                {
                    Console.WriteLine(@"Отменено пользователем");
                    return;
                }
            }
            ImportToolkit.DropTransaction(id);
            Console.WriteLine(@"Данные удалены.");
        }
    }
}
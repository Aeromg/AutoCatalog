using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoCatalogLib.Utils.Cli
{
    static class ArgumentsDictionary
    {
        public static ArgumentHeader Help { get; private set; }
        
        public static ArgumentHeader DropImport { get; private set; }
        public static ArgumentHeader DropRecords { get; private set; }
        public static ArgumentHeader DropEverything { get; private set; }

        public static ArgumentHeader Import { get; private set; }
        public static ArgumentHeader ImportId { get; private set; }
        public static ArgumentHeader RuleId { get; private set; }
        public static ArgumentHeader EmbedSource { get; private set; }

        public static ArgumentHeader ImportSource { get; private set; }

        public static ArgumentHeader ImportDb { get; private set; }
        public static ArgumentHeader ExportDb { get; private set; }

        public static ArgumentHeader ShowSources { get; private set; }
        public static ArgumentHeader ShowRules { get; private set; }
        public static ArgumentHeader ShowImports { get; private set; }

        public static ArgumentHeader Batch { get; private set; }

        public static ArgumentHeader IKnowWhatImDoing { get; set; }
        public static ArgumentHeader Silent { get; set; }

        private static bool _registered = false;

        static ArgumentsDictionary()
        {
            Register();
        }

        public static void Register()
        {
            if (_registered)
                return;
            
            _registered = true;

            MakeConstants();
            RegisterArgumentHeaders();
            SetDependencies();
            SetConflicts();
        }

        static void MakeConstants()
        {
            Help = new ArgumentHeader
            {
                Token = @"usage",
                Description = @"Выводит справку по аргументам командной строки",
                Value = ArgumentHeader.ArgumentValue.Empty
            };

            DropImport = new ArgumentHeader
            {
                Token = @"drop-import",
                Description = @"Удаляет записи, импортированные с указанным идентификатором импорта",
                Value = new ArgumentHeader.ArgumentValue
                {
                    Name = @"import_id",
                    Description = @"Идентификатор импорта"
                }
            };

            DropRecords = new ArgumentHeader
            {
                Token = @"drop-records",
                Description = @"Удаляет все импортированные записи",
                Value = ArgumentHeader.ArgumentValue.Empty
            };

            DropEverything = new ArgumentHeader
            {
                Token = @"drop-everything",
                Description = @"Очищает базу данных, удаляя все импортированные записи и настройки",
                Value = ArgumentHeader.ArgumentValue.Empty
            };

            IKnowWhatImDoing = new ArgumentHeader
            {
                Token = @"i-know-what-i-m-doing",
                Description = @"Отключает запрос подтверждения при деструктивных операциях",
                Value = ArgumentHeader.ArgumentValue.Empty
            };

            Import = new ArgumentHeader
            {
                Token = @"import",
                Description = @"Импортирует записи из произвольного источника",
                Value = new ArgumentHeader.ArgumentValue
                {
                    Name = @"source",
                    Description = @"Источник записей. Тип источника должен соответствовать типу правила"
                }
            };

            ImportId = new ArgumentHeader
            {
                Token = @"import-id",
                Description = @"Задает идентификатор импорта при импортировании записей из произвольного источника",
                IsSubArgument = true,
                Value = new ArgumentHeader.ArgumentValue
                {
                    Name = @"id",
                    Description = @"Идентификатор импорта"
                }
            };

            RuleId = new ArgumentHeader
            {
                Token = @"rule-id",
                Description = @"Задает идентификатор правила для импортирования записей из произвольного источника",
                IsSubArgument = true,
                Value = new ArgumentHeader.ArgumentValue
                {
                    Name = @"id",
                    Description = @"Идентификатор правила"
                }
            };

            EmbedSource = new ArgumentHeader
            {
                Token = @"embed-source",
                Description = @"Сохранить образ источника в базе данных",
                IsSubArgument = true,
                Value = ArgumentHeader.ArgumentValue.Empty
            };

            ImportSource = new ArgumentHeader
            {
                Token = @"import-source",
                Description = @"Производит импорт записей из источника с указанным идентификатором",
                Value = new ArgumentHeader.ArgumentValue
                {
                    Name = @"id",
                    Description = @"Идентификатор источника"
                }
            };

            ImportDb = new ArgumentHeader
            {
                Token = @"import-db",
                Description = @"Производит замену текущей базы данных",
                Value = new ArgumentHeader.ArgumentValue
                {
                    Name = @"file",
                    Description = @"Путь к файлу новой базы данных"
                }
            };

            ExportDb = new ArgumentHeader
            {
                Token = @"export-db",
                Description = @"Готовит базу данных к замене",
                Value = new ArgumentHeader.ArgumentValue
                {
                    Name = @"file",
                    Description = @"Путь к создаваемому файлу замены"
                }
            };

            ShowSources = new ArgumentHeader
            {
                Token = @"show-sources",
                Description = @"Выводит список зарегистрированных источников записей",
                Value = ArgumentHeader.ArgumentValue.Empty
            };

            ShowRules = new ArgumentHeader
            {
                Token = @"show-rules",
                Description = @"Выводит список правил импорта",
                Value = ArgumentHeader.ArgumentValue.Empty
            };

            ShowImports = new ArgumentHeader
            {
                Token = @"show-imports",
                Description = @"Выводит список идентификаторов импорта",
                Value = ArgumentHeader.ArgumentValue.Empty
            };

            Batch = new ArgumentHeader
            {
                Token = @"batch",
                Description = @"Выполняет указанный пакетный файл",
                Value = new ArgumentHeader.ArgumentValue
                {
                    Name = @"file",
                    Description = @"Пакетный файл"
                }
            };

            Silent = new ArgumentHeader
            {
                Token = @"silent",
                Description = @"Уменьшает количество выводимой на экран информации",
                Value = ArgumentHeader.ArgumentValue.Empty
            };
        }

        static void RegisterArgumentHeaders()
        {
            var constants = typeof(ArgumentsDictionary).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof (ArgumentHeader))
                .Select(p => p.GetValue(null)).Cast<ArgumentHeader>();

            ArgumentHeader.Headers.AddRange(constants);
        }

        static void SetDependencies()
        {
            ArgumentHeader.AddDependency(Import, ImportId);
            ArgumentHeader.AddDependency(Import, RuleId);
            ArgumentHeader.AddDependency(Import, EmbedSource);
        }

        static void SetConflicts()
        {
            foreach (var header in ArgumentHeader.Headers)
                ArgumentHeader.AddConflict(Help, header);

            foreach (var header in ArgumentHeader.Headers)
                ArgumentHeader.AddConflict(ImportDb, header);

            foreach (var header in ArgumentHeader.Headers)
                ArgumentHeader.AddConflict(ExportDb, header);

            ArgumentHeader.AddConflict(new [] { DropImport, DropRecords, DropEverything } );
            ArgumentHeader.AddConflict(new[] { Import, ImportSource });
            ArgumentHeader.AddConflict(new [] { ShowSources, ShowRules, ShowImports });
        }
    }
}

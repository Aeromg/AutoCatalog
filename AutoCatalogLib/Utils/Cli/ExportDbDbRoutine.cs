using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Persist;

namespace AutoCatalogLib.Utils.Cli
{
    class ExportDbDbRoutine : Routine
    {
        public override void Run()
        {
            var archive = Arguments.First(ArgumentsDictionary.ExportDb).Value;
            var dbFile = Config.GetDbPath();

            if (!File.Exists(dbFile))
                throw new FileNotFoundException(archive);

            Console.WriteLine(@"Формирование замены базы данных");
            Console.WriteLine(@"Файл замены: " + archive);

            Context.CloseAll();
            Compress(dbFile, archive);

            Console.WriteLine(@"Готово");
        }

        public static void Compress(string sourceFile, string targetFile)
        {
            if (File.Exists(targetFile))
                File.Delete(targetFile);

            using (var readerStream = File.OpenRead(sourceFile))
            {
                using (var writerStream = File.OpenWrite(targetFile))
                {
                    using (var writer = new GZipStream(writerStream, CompressionLevel.Optimal))
                    {
                        readerStream.CopyTo(writer);
                        writer.Flush();
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCatalogLib.Utils.Cli
{
    class ImportDbRoutine : Routine
    {
        public override void Run()
        {
            var archive = Arguments.First(ArgumentsDictionary.ImportDb).Value;

            if (!File.Exists(archive))
                throw new FileNotFoundException(archive);

            Console.WriteLine(@"Замена базы данных");
            Console.WriteLine(@"Файл замены: " + archive);

            var dbFile = Config.GetDbPath();
            Decompress(archive, dbFile);

            Console.WriteLine(@"Готово");
        }

        public static void Decompress(string sourceFile, string targetFile)
        {
            var tmpFile = Path.GetTempFileName();

            if (File.Exists(tmpFile))
                File.Delete(tmpFile);

            using (var readerStream = File.OpenRead(sourceFile))
            {
                using (var reader = new GZipStream(readerStream, CompressionMode.Decompress))
                {
                    using (var writer = File.OpenWrite(tmpFile))
                    {
                        reader.CopyTo(writer);
                        writer.Flush();
                    }
                }
            }

            if (File.Exists(targetFile))
                File.Delete(targetFile);

            File.Move(tmpFile, targetFile);
        }
    }
}

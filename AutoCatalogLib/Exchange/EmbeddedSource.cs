using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoCatalogLib.Business;
using AutoCatalogLib.Persist;
using AutoCatalogLib.Persist.BusinessModels.Catalog;
using AutoCatalogLib.Utils;
using NLog;

namespace AutoCatalogLib.Exchange
{
    public class EmbeddedSource : ISource, IDisposable
    {
        private string _file;

        public string Location { get; set; }

        public string File {
            get
            {
                if (String.IsNullOrEmpty(_file))
                    throw new Exception("Embedded file must be opened before first use!");

                return _file;
            }
        }

        public void Open()
        {
            _file = CreateTempFile();
        }

        private string CreateTempFile()
        {
            var target = SourceLocator.GetTargetLocation(Location);
            var fileEntity = Context.Default.EmbeddedBlobs.FirstOrDefault(f => f.Name == target);

            if (fileEntity == null)
                throw new FileNotFoundException();

            var tempFilePath = Path.GetTempFileName();

            System.IO.File.WriteAllBytes(tempFilePath, fileEntity.Data);

            return tempFilePath;
        }

        public void Close()
        {
            if (!System.IO.File.Exists(File)) 
                return;

            int tryNum = 0;

            Task.Factory.StartNew(() =>
            {
                bool done = false;

                do
                {
                    try
                    {
                        tryNum++;
                        System.IO.File.Delete(File);
                        done = true;
                    }
                    catch (Exception ex)
                    {
                        if (tryNum < 10)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }

                        Log.Exception(ex);
                        return;
                    }
                } while (!done);
            });
        }

        public static EmbeddedSource FromSource(ISource source)
        {
            var file = EmbeddedBlob.FromFile(source.File);
            file.Save();

            return new EmbeddedSource
            {
                Location = SourceLocator.EmbeddedProtocol + file.Name
            };
        }

        public void Dispose()
        {
            Close();
        }
    }
}

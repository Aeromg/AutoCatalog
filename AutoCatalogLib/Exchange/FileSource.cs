using System.IO;
using AutoCatalogLib.Business;

namespace AutoCatalogLib.Exchange
{
    public class FileSource : ISource
    {
        public string Location { get; set; }

        public string File
        {
            get { return SourceLocator.GetTargetLocation(Location); }
        }

        public void Open()
        {
            if(!System.IO.File.Exists(File))
                throw new FileNotFoundException(File);
        }

        public void Close() { }
    }
}

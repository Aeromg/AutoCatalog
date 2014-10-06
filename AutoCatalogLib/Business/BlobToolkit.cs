using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Persist;
using AutoCatalogLib.Persist.BusinessModels.Catalog;

namespace AutoCatalogLib.Business
{
    public static class BlobToolkit
    {
        public static EmbeddedBlob CreateFromFile(string file, ImportIdentificator importIdentificator)
        {
            if(importIdentificator.IsNew)
                throw new Exception("Идентификатор импорта должен быть предварительно записан");

            var blob = EmbeddedBlob.FromFile(file);
            blob.ImportIdentificatorId = importIdentificator.Id;
            using (var context = new Context())
            {
                context.EmbeddedBlobs.AddOrAttach(blob);
                context.SaveChanges();
            }

            return blob;
        }
    }
}

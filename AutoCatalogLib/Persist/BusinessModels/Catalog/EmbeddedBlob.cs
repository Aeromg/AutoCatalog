using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using AutoCatalogLib.Persist.Generic;
using AutoCatalogLib.Utils;

namespace AutoCatalogLib.Persist.BusinessModels.Catalog
{
    public class EmbeddedBlob : Entity
    {
        public string Name { get; set; }

        public long ImportIdentificatorId { get; set; }

        public byte[] Data { get; set; }

        public static EmbeddedBlob FromFile(string file)
        {
            using (var fileReader = File.OpenRead(file))
            {
                var data = new byte[fileReader.Length];
                fileReader.Read(data, 0, data.Length);

                return new EmbeddedBlob()
                {
                    Name = StringGenerator.GetRandomString(8) + "_" + Path.GetFileName(file),
                    Data = data
                };
            }
        }
    }
}
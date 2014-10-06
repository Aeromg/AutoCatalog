using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Persist
{
    public class ConfigElement : Entity
    {
        public string Name { get; set; }
        public byte[] Value { get; set; }

        public T GetStoredValue<T>()
        {
            return (T)ByteArrayToObject(Value);
        }

        public object GetStoredValue()
        {
            if (Value == null || Value.Length == 0)
                return null;

            return ByteArrayToObject(Value);
        }

        public void SetStoredValue(object value)
        {
            Value = ObjectToByteArray(value);
        }

        private static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        private static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = binForm.Deserialize(memStream);
            return obj;
        }
    }
}

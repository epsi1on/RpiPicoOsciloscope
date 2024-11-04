using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOsciloscope.UI
{
    public static class SerializationUtil
    {
        public static byte[] Serialize(object obj)
        {
            using (var str = new MemoryStream())
            {
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(str, obj);

                return str.ToArray();
            }
        }

        public static object DeSerialize(byte[] data)
        {
            using (var str = new MemoryStream(data))
            {
                BinaryFormatter bin = new BinaryFormatter();
                var tmp = bin.Deserialize(str);

                return tmp;
            }
        }

        /*
        private class SerializableDicWrapper : ISerializable
        {
            public string TypeName;
            public Array Keys;
            public Array Values;

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("TypeName", TypeName);
                info.AddValue("Keys", Keys);
                info.AddValue("Values", Values);


                throw new NotImplementedException();
            }
        }
        */

    }
}

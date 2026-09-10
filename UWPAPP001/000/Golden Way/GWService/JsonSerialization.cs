using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWService
{
    public class JsonSerialization
    {
        public static String Serializate(Object toSerializate)
        {
            Parameter p = new Parameter() { Type = toSerializate.GetType().FullName, ObjectS = JsonConvert.SerializeObject(toSerializate) };
            string json = JsonConvert.SerializeObject(p);
            return json;
        }

        public static Object Deserializate(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                String json = Encoding.UTF8.GetString(data);
                Parameter p = JsonConvert.DeserializeObject<Parameter>(json);
                Object o = JsonConvert.DeserializeObject(p.ObjectS, Type.GetType(p.Type));
                return o;
            }
            else
            {
                return String.Empty;
            }
        }
    }
}

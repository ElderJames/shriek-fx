using Newtonsoft.Json;
using System;
using System.Text;

namespace Shriek.ServiceProxy.Tcp.Serialization.Json
{
    public class JsonSerializer : ISerializer
    {
        public object Deserialize(Type type, byte[] data)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), type);
        }

        public T Deserialize<T>(byte[] data)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
        }

        public byte[] Serialize(object obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }
    }
}
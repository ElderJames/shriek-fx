using ProtoBuf;
using System;
using System.IO;

namespace Shriek.ServiceProxy.Tcp.Serialization.Protobuf
{
    public class ProtoSerializer : ISerializer
    {
        public object Deserialize(Type type, byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                return Serializer.Deserialize(type, ms);
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            return (T)Deserialize(typeof(T), data);
        }

        public byte[] Serialize(object obj)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
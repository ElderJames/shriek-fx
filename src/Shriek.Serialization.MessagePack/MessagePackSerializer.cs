using System;

namespace Shriek.Serialization.MessagePack
{
    public class MessagePackSerializer : ISerializer
    {
        public MessagePackSerializer()
        {
        }

        public dynamic Deserialize(Type objType, string str)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(string str)
        {
            throw new NotImplementedException();
        }

        public string Serialize(object obj)
        {
            throw new NotImplementedException();
        }

        public dynamic SerializeDynamic(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
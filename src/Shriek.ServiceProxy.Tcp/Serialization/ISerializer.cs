using System;

namespace Shriek.ServiceProxy.Tcp.Serialization
{
    public interface ISerializer
    {
        byte[] Serialize(object obj);

        T Deserialize<T>(byte[] data);

        object Deserialize(Type type, byte[] data);
    }
}
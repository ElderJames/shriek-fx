using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shriek.Serialization
{
    public interface ISerializer
    {
        dynamic SerializeDynamic(object obj);

        string Serialize(object obj);

        dynamic Deserialize(Type objType, string str);

        T Deserialize<T>(string str);
    }
}
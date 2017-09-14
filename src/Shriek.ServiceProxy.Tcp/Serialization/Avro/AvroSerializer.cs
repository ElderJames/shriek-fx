//using TcpServiceCore.Protocol;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using MsAvro = Microsoft.Hadoop.Avro;

//namespace TcpServiceCore.Serialization.Avro
//{
//    public class AvroSerializer : ISerializer
//    {
//        public object Deserialize(Type type, byte[] data)
//        {
//            var serializer = MsAvroWrapper.GetSerializer(type);
//            return serializer.Deserialize(data);
//        }

//        public T Deserialize<T>(byte[] data)
//        {
//            return (T)Deserialize(typeof(T), data);
//        }

//        public byte[] Serialize(object obj)
//        {
//            var serializer = MsAvroWrapper.GetSerializer(obj.GetType());
//            return serializer.Serialize(obj);
//        }
//    }
//}

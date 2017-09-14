//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using MsAvro = Microsoft.Hadoop.Avro;

//namespace TcpServiceCore.Serialization.Avro
//{
//    class MsAvroWrapper
//    {
//        static MsAvro.AvroSerializerSettings settings = new MsAvro.AvroSerializerSettings();
//        static Dictionary<Type, MsAvroWrapper> Serializers = new Dictionary<Type, MsAvroWrapper>();

//        static MsAvroWrapper()
//        {
//            settings.UseCache = true;
//        }


//        public static MsAvroWrapper GetSerializer(Type type)
//        {
//            try
//            {
//                return Serializers[type];
//            }
//            catch
//            {
//                lock (Serializers)
//                {
//                    if (Serializers.Keys.Any(x => x == type) == false)
//                    {
//                        Serializers.Add(type, new MsAvroWrapper(type));
//                    }
//                }
//                return Serializers[type];
//            }
//        }

//        object serializer;
//        MethodInfo serializeMethod;
//        MethodInfo deserializeMethod;
//        private MsAvroWrapper(Type type)
//        {
//            var create = typeof(MsAvro.AvroSerializer).GetMethod("Create", 
//                             new Type[] { typeof(MsAvro.AvroSerializerSettings) })
//                            .MakeGenericMethod(type);

//            this.serializer = create.Invoke(null, new object[] { settings });

//            Type serializerType = this.serializer.GetType();

//            this.serializeMethod = serializerType.GetMethod("Serialize", 
//                                        new Type[] {
//                                                typeof(Stream),
//                                                type
//                                            });
//            this.deserializeMethod = serializerType.GetMethod("Deserialize", 
//                                        new Type[] {
//                                                typeof(Stream)
//                                            });
//        }

//        public object Deserialize(byte[] data)
//        {
//            using (var ms = new MemoryStream(data))
//            {
//                return this.deserializeMethod.Invoke(this.serializer, new object[] { ms });
//            }
//        }

//        public byte[] Serialize(object obj)
//        {
//            using (var ms = new MemoryStream())
//            {
//                this.serializeMethod.Invoke(this.serializer, new object[] { ms, obj });
//                return ms.ToArray();
//            }
//        }
//    }
//}

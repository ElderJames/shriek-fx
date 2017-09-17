using Shriek.ServiceProxy.Tcp.Buffering;
using Shriek.ServiceProxy.Tcp.Exceptions;
using Shriek.ServiceProxy.Tcp.Serialization;
using Shriek.ServiceProxy.Tcp.Serialization.Protobuf;
using Shriek.ServiceProxy.Tcp.Tools;

namespace Shriek.ServiceProxy.Tcp
{
    public static class Global
    {
        public static ISerializer Serializer { get; set; }
        public static ExceptionHandler ExceptionHandler { get; set; }
        public static IMsgIdProvider IdProvider { get; set; }
        public static BufferManagerFactory BufferManagerFactory { get; set; }

        static Global()
        {
            Serializer = new ProtoSerializer();
            ExceptionHandler = new ExceptionHandler();
            IdProvider = new SimpleIdProvider();
            BufferManagerFactory = new BufferManagerFactory();
        }
    }
}
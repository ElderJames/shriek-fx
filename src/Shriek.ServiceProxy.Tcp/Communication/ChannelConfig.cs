using System;

namespace Shriek.ServiceProxy.Tcp.Communication
{
    public class ChannelConfig
    {
        //Buffers
        public int MaxBufferSize { get; set; }

        public int MaxBufferPoolSize { get; set; }

        //Socket
        public TimeSpan ReceiveTimeout { get; set; }

        public TimeSpan SendTimeout { get; set; }
        public bool NoDelay { get; set; }
        public int ReceiveBufferSize { get; set; }
        public int SendBufferSize { get; set; }

        public ChannelConfig()
        {
            this.MaxBufferSize = 65536;
            this.MaxBufferPoolSize = this.MaxBufferSize * 10;

            this.SendTimeout = TimeSpan.FromSeconds(10);
            this.ReceiveTimeout = TimeSpan.FromSeconds(60);

            this.NoDelay = true;
        }
    }
}
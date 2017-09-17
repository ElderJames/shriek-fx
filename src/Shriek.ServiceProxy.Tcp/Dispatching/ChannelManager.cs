using Shriek.ServiceProxy.Tcp.Buffering;
using Shriek.ServiceProxy.Tcp.Communication;

namespace Shriek.ServiceProxy.Tcp.Dispatching
{
    public class ChannelManager
    {
        public readonly ChannelConfig Config;
        public readonly IBufferManager BufferManager;
        public readonly ContractDescription Contract;

        public ChannelManager(ContractDescription contract, ChannelConfig config)
        {
            this.Contract = contract;
            this.Config = config;
            this.BufferManager = this.Contract.CreateBufferManager(config);
        }
    }
}
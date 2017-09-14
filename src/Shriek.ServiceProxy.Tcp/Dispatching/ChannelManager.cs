using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TcpServiceCore.Buffering;
using TcpServiceCore.Communication;

namespace TcpServiceCore.Dispatching
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

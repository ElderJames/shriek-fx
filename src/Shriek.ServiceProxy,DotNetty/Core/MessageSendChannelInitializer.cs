using System;
using DotNetty.Transport.Channels;
using Shriek.ServiceProxy.DotNetty.Serialize;

namespace Shriek.ServiceProxy.DotNetty.Core
{
	public class MessageSendChannelInitializer<T> : ChannelInitializer<T>
		where T : IChannel
	{
		private readonly Action<T> initializationAction;
		private ISerializer protocol;
		private ISerializeFrame frame = new MessageSendSerializeFrame();

		public MessageSendChannelInitializer(Action<T> initializationAction)
		{
			this.initializationAction = initializationAction;
		}

		public MessageSendChannelInitializer<T> BuildSerializeProtocol(ISerializer protocol)
		{
			this.protocol = protocol;
			return this;
		}

		protected override void InitChannel(T channel)
		{
			this.initializationAction(channel);
			IChannelPipeline pipeline = channel.Pipeline;
			this.frame.Select(this.protocol, pipeline);
		}
	}
}
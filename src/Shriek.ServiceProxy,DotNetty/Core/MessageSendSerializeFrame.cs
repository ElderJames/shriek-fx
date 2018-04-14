using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Shriek.ServiceProxy.DotNetty.Serialize;

namespace Shriek.ServiceProxy.DotNetty.Core
{
	public class MessageSendSerializeFrame : ISerializeFrame
	{
		public void Select(ISerializer protocol, IChannelPipeline pipeline)
		{
			if (protocol.GetType() == typeof(DefaultSerializer))
			{
				pipeline.AddLast(new LengthFieldPrepender(2));
				pipeline.AddLast(new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));
				pipeline.AddLast(new MessageSendHandler(protocol));
			} //TODO use pf or other protocal ,now use binary serializer as default

			RpcServerLoader.Instance.SetMessageSendHandler(pipeline.Get<MessageSendHandler>());
		}
	}
}
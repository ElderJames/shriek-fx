using System;
using System.Collections.Concurrent;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Shriek.ServiceProxy.DotNetty.Model;
using Shriek.ServiceProxy.DotNetty.Serialize;

namespace Shriek.ServiceProxy.DotNetty.Core
{
	public class MessageSendHandler : ChannelHandlerAdapter
	{
		private IByteBuffer requestMessage;
		private readonly ISerializer serializer;
		private readonly ConcurrentDictionary<string, MessageSendCallBack> callBackActions = new ConcurrentDictionary<string, MessageSendCallBack>();
		private IChannelHandlerContext channelContext;

		public MessageSendHandler(ISerializer serializer)
		{
			this.serializer = serializer;
		}

		public override void ChannelRegistered(IChannelHandlerContext context)
		{
			base.ChannelRegistered(context);
			this.channelContext = context;
		}

		public override void ChannelActive(IChannelHandlerContext context)
		{
		}

		public override void ChannelRead(IChannelHandlerContext context, object message)
		{
			if (message is IByteBuffer byteBuffer)
			{
				var response = (SocketResponseMessage)this.serializer.Deserialize(byteBuffer.Array);

				MessageSendCallBack callBack = this.callBackActions[response.MessageId];
				callBack.Over(new SocketResponseMessage());
				Console.WriteLine("Received from server: " + response.MessageId);
			}
		}

		public override void ChannelReadComplete(IChannelHandlerContext context)
		{
			context.Flush();
		}

		public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
		{
			context.CloseAsync();
		}

		public MessageSendCallBack SendRequest(SocketRequestMessage request)
		{
			this.requestMessage = Unpooled.Buffer(MessageSendSettings.Size);
			var callBack = new MessageSendCallBack(request);
			this.callBackActions.TryAdd(request.MessageId, callBack);
			this.requestMessage.WriteBytes(this.serializer.Serialize(request));
			this.channelContext.WriteAndFlushAsync(this.requestMessage);

			return callBack;
		}
	}
}
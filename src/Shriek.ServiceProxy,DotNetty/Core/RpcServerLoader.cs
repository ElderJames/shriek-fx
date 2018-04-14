using System;
using System.Net;
using DotNetty.Transport.Channels;
using Shriek.ServiceProxy.DotNetty.Serialize;

namespace Shriek.ServiceProxy.DotNetty.Core
{
	public class RpcServerLoader
	{
		private static readonly Lazy<RpcServerLoader> lazy = new Lazy<RpcServerLoader>(() => new RpcServerLoader());
		private MessageSendHandler messageSendHandler = null;
		private readonly IEventLoopGroup eventLoopGroup = new MultithreadEventLoopGroup();
		public static RpcServerLoader Instance { get { return lazy.Value; } }

		private RpcServerLoader()
		{
		}

		public async void Load(string serverAddr, int port, ISerializer serializer)
		{
			var ipEndPoint = new IPEndPoint(IPAddress.Parse(serverAddr), port);
			var connector = new MessageSendConnector(this.eventLoopGroup, ipEndPoint, serializer);

			await connector.Call(); // TODO  asynchronously run
		}

		public void SetMessageSendHandler(MessageSendHandler handler)
		{
			this.messageSendHandler = handler;  //TODO add thread safe lock
		}

		public MessageSendHandler GetMessageSendHandler()
		{
			return this.messageSendHandler;
		}
	}
}
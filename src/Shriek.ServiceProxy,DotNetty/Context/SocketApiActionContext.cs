using Shriek.ServiceProxy.Abstractions.Context;
using Shriek.ServiceProxy.DotNetty.Model;

namespace Shriek.ServiceProxy.DotNetty.Context
{
	public class SocketApiActionContext : ApiActionContext
	{
		public SocketRequestMessage RequestMessage { get; set; }

		public SocketResponseMessage ResponseMessage { get; set; }
	}
}
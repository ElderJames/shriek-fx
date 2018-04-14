using System;

namespace Shriek.ServiceProxy.DotNetty.Model
{
	[Serializable]
	public class SocketResponseMessage
	{
		public string MessageId { get; set; }

		public string Error { get; set; }

		public object Result { get; set; }
	}
}
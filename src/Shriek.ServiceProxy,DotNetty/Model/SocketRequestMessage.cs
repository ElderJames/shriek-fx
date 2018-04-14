using System;

namespace Shriek.ServiceProxy.DotNetty.Model
{
	[Serializable]
	public class SocketRequestMessage
	{
		public string MessageId { get; set; }

		public string ClassName { get; set; }

		public string MethodName { get; set; }

		public Type[] ParamTypes { get; set; }

		public object[] Parameters { get; set; }
	}
}
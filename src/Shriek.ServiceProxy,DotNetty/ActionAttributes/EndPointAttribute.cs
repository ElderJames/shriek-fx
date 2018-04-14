using System;
using System.Net;

namespace Shriek.ServiceProxy.DotNetty.ActionAttributes
{
	public class EndPointAttribute : Attribute
	{
		public EndPoint EndsPoint { get; }

		public EndPointAttribute(EndPoint endPoint)
		{
			this.EndsPoint = endPoint;
		}
	}
}
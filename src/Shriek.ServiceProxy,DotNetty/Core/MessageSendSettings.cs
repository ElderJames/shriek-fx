// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;

namespace Shriek.ServiceProxy.DotNetty.Core
{
	public static class MessageSendSettings
	{
		public static IPAddress Host { get; }

		public static int Port { get; }

		public static int Size { get; }
	}
}
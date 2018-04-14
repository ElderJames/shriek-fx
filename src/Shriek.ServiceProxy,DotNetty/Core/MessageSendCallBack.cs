using System.Threading;
using Shriek.ServiceProxy.DotNetty.Model;

namespace Shriek.ServiceProxy.DotNetty.Core
{
	public class MessageSendCallBack
	{
		private SocketRequestMessage request;
		private SocketResponseMessage response;
		private readonly object _lock = new object();
		private SpinLock spinlock = new SpinLock();

		public MessageSendCallBack(SocketRequestMessage request)
		{
			this.request = request;
		}

		public SocketResponseMessage Start()
		{
			bool lockTaken = false;
			try
			{
				lock (this._lock)
				{
					this.spinlock.TryEnter(1000 * 10, ref lockTaken); // set locker timeout
					return this.response;
				}
			}
			finally
			{
				//TODO  maybe some callback for caller
				//Console.WriteLine("locked response ");
			}
		}

		public void Over(SocketResponseMessage response)
		{
			lock (this._lock)
			{
				this.spinlock.Exit(); // release the lock
				this.response = response;
			}
		}
	}
}
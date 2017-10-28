using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 表示数据包id提供者
    /// </summary>
    internal class PacketIdProvider
    {
        /// <summary>
        /// 基准值
        /// </summary>
        private long id = 0L;

        /// <summary>
        /// 获取标识符
        /// 每获取一次自增1
        /// </summary>
        /// <returns></returns>
        public long NewId()
        {
            return Interlocked.Increment(ref this.id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Tcp.Streams
{
    /// <summary>
    /// 表示会话收到的数据流
    /// </summary>
    public class SessionStream : MemoryStream
    {
        /// <summary>
        /// 会话收到的数据流
        /// </summary>
        public SessionStream()
            : base()
        {
        }
    }
}
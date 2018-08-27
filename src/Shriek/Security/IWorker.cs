using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shriek.Security
{
    public interface IWorker
    {
        /// <summary>
        /// 操作者名字
        /// </summary>
        string WorkerName { get; }

        /// <summary>
        /// 操作者Id
        /// </summary>
        long WorkerId { get; }
    }
}
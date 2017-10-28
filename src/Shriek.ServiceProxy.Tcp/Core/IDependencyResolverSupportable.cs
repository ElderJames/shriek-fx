using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 定义支持依赖注入功能的接口
    /// </summary>
    public interface IDependencyResolverSupportable
    {
        /// <summary>
        /// 获取或设置依赖注入提供者
        /// </summary>
        IDependencyResolver DependencyResolver { get; set; }
    }
}

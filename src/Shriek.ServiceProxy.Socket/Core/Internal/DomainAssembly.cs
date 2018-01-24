using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shriek.ServiceProxy.Socket.Core.Internal
{
    /// <summary>
    /// 当前程序域内的程序集
    /// </summary>
    internal static class DomainAssembly
    {
        /// <summary>
        /// 获取程序域内所有第三方程序集
        /// 不包含当前程序集和全局程序集
        /// </summary>
        /// <returns></returns>
        public static List<Assembly> GetAssemblies()
        {
            var current = Assembly.GetAssembly(typeof(DomainAssembly));
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(item => item.GlobalAssemblyCache == false)
                .Where(item => item != current)
                .ToList();
        }
    }
}
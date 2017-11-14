using Shriek.ServiceProxy.Abstractions.Context;
using System;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Abstractions.Attributes
{
    /// <summary>
    /// 表示请求Api过滤器特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public abstract class ApiActionFilterAttribute : Attribute
    {
        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual Task OnBeginRequestAsync(ApiActionContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual Task OnEndRequestAsync(ApiActionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
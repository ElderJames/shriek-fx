using System;
using System.Threading.Tasks;
using Shriek.ServiceProxy.Abstractions.Context;

namespace Shriek.ServiceProxy.Abstractions.Attributes
{
    /// <summary>
    /// 表示属性关联的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class ApiParameterAttribute : Attribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        public abstract Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter);
    }
}
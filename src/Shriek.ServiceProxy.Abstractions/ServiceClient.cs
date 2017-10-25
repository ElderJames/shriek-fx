using System;
using System.Threading.Tasks;
using Shriek.ServiceProxy.Abstractions.Context;

namespace Shriek.ServiceProxy.Abstractions
{
    public interface IServiceClient
    {
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task SendAsync(ApiActionContext context);

        /// <summary>
        /// Json格式化器
        /// </summary>
        IJsonFormatter JsonFormatter { get; }

        /// <summary>
        /// 请求服务器地址
        /// </summary>
        Uri RequestHost { get; }
    }
}
using System;

namespace Shriek.ServiceProxy.Tcp.Core
{
    /// <summary>
    /// 定义可简化服务位置和依赖关系解析的方法
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// 解析支持任意对象创建的一次注册的服务
        /// </summary>
        /// <param name="serviceType">所请求的服务或对象的类型</param>
        /// <returns></returns>
        object GetService(Type serviceType);

        /// <summary>
        /// 结束服务实例的生命
        /// </summary>
        /// <param name="service">服务实例</param>
        void TerminateService(IDisposable service);
    }
}
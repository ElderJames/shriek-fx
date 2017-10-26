using System;
using AspectCore.Configuration;
using AspectCore.DynamicProxy;

namespace Shriek.ServiceProxy.Http
{
    /// <summary>
    /// 服务提供器
    /// </summary>
    public static class ServiceProvider
    {
        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <typeparam name="TInterface">请求接口类型</typeparam>
        /// <returns></returns>
        public static TInterface GetHttpService<TInterface>() where TInterface : class
        {
            return GetHttpService(typeof(TInterface), null) as TInterface;
        }

        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <typeparam name="TInterface">请求接口类型</typeparam>
        /// <param name="host">服务主机地址，效果与HttpHostAttribute一致</param>
        /// <returns></returns>
        public static TInterface GetHttpService<TInterface>(string host) where TInterface : class
        {
            return GetHttpService(typeof(TInterface), host) as TInterface;
        }

        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <param name="obj">请求接口类型</param>
        /// <param name="host">服务主机地址</param>
        /// <returns></returns>
        public static object GetHttpService(Type obj, string host)
        {
            if (!obj.IsInterface)
            {
                throw new ArgumentException(obj.Name + "不是接口类型");
            }
            return GeneratoProxy(obj, host);
        }

        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <param name="type">请求接口类型</param>
        /// <param name="host">服务跟路径</param>
        /// <returns></returns>
        private static object GeneratoProxy(Type type, string host)
        {
            var proxyGeneratorBuilder = new ProxyGeneratorBuilder();
            proxyGeneratorBuilder.Configure(config =>
            {
                config.Interceptors.AddDelegate(async (ctx, next) =>
                {
                    await next(ctx);
                });

                config.Interceptors.AddTyped<HttpApiClient>(new object[] { host });
            });

            var proxyGenerator = proxyGeneratorBuilder.Build();

            return proxyGenerator.CreateInterfaceProxy(type);
        }
    }
}
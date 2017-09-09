using System;
using AspectCore.Configuration;
using AspectCore.DynamicProxy;

namespace Shriek.ServiceProxy.Http
{
    public static class ServiceAdapter
    {
        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <typeparam name="TInterface">请求接口</typeparam>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static TInterface GetHttpApi<TInterface>() where TInterface : class
        {
            if (!typeof(TInterface).IsInterface)
            {
                throw new ArgumentException(typeof(TInterface).Name + "不是接口类型");
            }

            return GeneratoProxy<TInterface>(null);
        }

        public static object GetHttpApi(Type obj, string host)
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
        /// <typeparam name="TInterface">请求接口</typeparam>
        /// <param name="host">服务跟路径，效果与HttpHostAttribute一致</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static TInterface GetHttpApi<TInterface>(string host) where TInterface : class
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException();
            }

            if (!typeof(TInterface).IsInterface)
            {
                throw new ArgumentException(typeof(TInterface).Name + "不是接口类型");
            }

            return GeneratoProxy<TInterface>(host);
        }

        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <typeparam name="TInterface">请求接口</typeparam>
        /// <param name="host">服务跟路径</param>
        /// <param name="interceptor">拦截器</param>
        /// <returns></returns>
        private static TInterface GeneratoProxy<TInterface>(string host) where TInterface : class
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

            IProxyGenerator proxyGenerator = proxyGeneratorBuilder.Build();

            return proxyGenerator.CreateInterfaceProxy<TInterface>();
        }

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

            IProxyGenerator proxyGenerator = proxyGeneratorBuilder.Build();

            return proxyGenerator.CreateInterfaceProxy(type);
        }
    }
}
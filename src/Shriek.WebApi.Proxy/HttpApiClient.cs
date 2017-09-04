﻿using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Net.Http;

namespace Shriek.WebApi.Proxy
{
    /// <summary>
    /// 表示web api请求客户端
    /// </summary>
    public class HttpApiClient : IInterceptor, IDisposable
    {
        /// <summary>
        /// 代理生成器
        /// </summary>
        private static readonly ProxyGenerator generator = new ProxyGenerator();

        /// <summary>
        /// 获取或设置http客户端
        /// </summary>
        public IHttpClient HttpClient { get; set; }

        /// <summary>
        /// 获取或设置json解析工具
        /// </summary>
        public IJsonFormatter JsonFormatter { get; set; }

        /// <summary>
        /// web api请求客户端
        /// </summary>
        public HttpApiClient()
        {
            HttpClient = new HttpClientAdapter(new HttpClient());
            this.JsonFormatter = new DefaultJsonFormatter();
        }

        public HttpApiClient(IHttpClient httpClient)
        {
            HttpClient = httpClient ?? new HttpClientAdapter(new HttpClient());
            this.JsonFormatter = new DefaultJsonFormatter();
        }

        /// <summary>
        /// web api请求客户端
        /// </summary>
        /// <param name="httpClient">关联的http客户端</param>
        public HttpApiClient(HttpClient httpClient)
        {
            HttpClient = new HttpClientAdapter(httpClient ?? new HttpClient());
            this.JsonFormatter = new DefaultJsonFormatter();
        }

        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <typeparam name="TInterface">请求接口</typeparam>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public TInterface GetHttpApi<TInterface>() where TInterface : class
        {
            if (!typeof(TInterface).IsInterface)
            {
                throw new ArgumentException(typeof(TInterface).Name + "不是接口类型");
            }

            return GeneratoProxy<TInterface>(null, this);
        }

        public object GetHttpApi(Type obj, string host)
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException();
            }

            if (!obj.IsInterface)
            {
                throw new ArgumentException(obj.Name + "不是接口类型");
            }
            return GeneratoProxy(obj, host, this);
        }

        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <typeparam name="TInterface">请求接口</typeparam>
        /// <param name="host">服务跟路径，效果与HttpHostAttribute一致</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public TInterface GetHttpApi<TInterface>(string host) where TInterface : class
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException();
            }

            if (!typeof(TInterface).IsInterface)
            {
                throw new ArgumentException(typeof(TInterface).Name + "不是接口类型");
            }

            return GeneratoProxy<TInterface>(host, this);
        }

        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <typeparam name="TInterface">请求接口</typeparam>
        /// <param name="host">服务跟路径</param>
        /// <param name="interceptor">拦截器</param>
        /// <returns></returns>
        private static TInterface GeneratoProxy<TInterface>(string host, IInterceptor interceptor) where TInterface : class
        {
            var option = new ProxyGenerationOptions();
            if (!string.IsNullOrEmpty(host))
            {
                var ctor = typeof(HttpHostAttribute).GetConstructors().FirstOrDefault();
                var hostAttribute = new CustomAttributeInfo(ctor, new object[] { host });
                option.AdditionalAttributes.Add(hostAttribute);
            }
            return generator.CreateInterfaceProxyWithoutTarget<TInterface>(option, interceptor);
        }

        private static object GeneratoProxy(Type type, string host, IInterceptor interceptor)
        {
            var option = new ProxyGenerationOptions();
            if (!string.IsNullOrEmpty(host))
            {
                var ctor = typeof(HttpHostAttribute).GetConstructors().FirstOrDefault();
                var hostAttribute = new CustomAttributeInfo(ctor, new object[] { host });
                option.AdditionalAttributes.Add(hostAttribute);
            }
            return generator.CreateInterfaceProxyWithoutTarget(type, option, interceptor);
        }

        /// <summary>
        /// 方法拦截
        /// </summary>
        /// <param name="invocation">拦截内容</param>
        void IInterceptor.Intercept(IInvocation invocation)
        {
            var context = CastleContext.From(invocation);
            var actionContext = new ApiActionContext
            {
                HttpApiClient = this,
                RequestMessage = new HttpRequestMessage(),
                HostAttribute = context.HostAttribute,
                ApiReturnAttribute = context.ApiReturnAttribute,
                ApiActionFilterAttributes = context.ApiActionFilterAttributes,
                ApiActionDescriptor = context.ApiActionDescriptor.Clone() as ApiActionDescriptor
            };

            var parameters = actionContext.ApiActionDescriptor.Parameters;
            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i].Value = invocation.Arguments[i];
            }

            var apiAction = context.ApiActionDescriptor;
            invocation.ReturnValue = apiAction.Execute(actionContext);
        }

        /// <summary>
        /// 释放相关资源
        /// </summary>
        public void Dispose()
        {
            this.HttpClient.Dispose();
        }
    }
}
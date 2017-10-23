using System;
using System.Collections.Generic;

namespace Shriek.ServiceProxy.Abstractions
{
    public class WebApiProxyOptions
    {
        public ICollection<IWebApiProxy> WebApiProxies { get; } = new List<IWebApiProxy>();

        public ICollection<KeyValuePair<string, Type>> RegisteredServices { get; } = new List<KeyValuePair<string, Type>>();

        public string ProxyHost { get; set; }

        /// <summary>
        /// 使用对象类型注册Http请求服务，会自动注册该对象类型所在程序集中的服务接口（必需有标记HttpMethod特性）
        /// </summary>
        /// <typeparam name="TWebApiProxy">应契约接口的WebApiProxy配置对象类型，会自动注册该文件所在程序集中的服务接口（必需有标记HttpMethod特性）</typeparam>
        /// <param name="baseUrl">服务器地址，不填时需要在契约接口标记HttpHost特性</param>
        public void AddWebApiProxy<TWebApiProxy>(string baseUrl = null) where TWebApiProxy : WebApiProxy
        {
            if (Activator.CreateInstance(typeof(TWebApiProxy), baseUrl) is WebApiProxy proxy)
                WebApiProxies.Add(proxy);
        }

        /// <summary>
        /// 使用契约接口注册Http请求服务
        /// </summary>
        /// <typeparam name="TService">契约接口类型</typeparam>
        /// <param name="baseUrl">服务器地址，不填时需要在契约接口标记HttpHost特性</param>
        public void AddService<TService>(string baseUrl = null)
        {
            if (!typeof(TService).IsInterface)
                throw new ArgumentOutOfRangeException($"{typeof(TService).Name} is not a interface.");

            RegisteredServices.Add(new KeyValuePair<string, Type>(baseUrl, typeof(TService)));
        }
    }
}
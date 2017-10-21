using System;
using System.Collections.Generic;

namespace Shriek.ServiceProxy.Abstractions
{
    public class WebApiProxyOptions
    {
        public ICollection<IWebApiProxy> WebApiProxies { get; } = new List<IWebApiProxy>();

        public ICollection<KeyValuePair<string, Type>> RegisteredServices { get; } = new KeyValuePair<string, Type>[0];

        /// <summary>
        /// 注册Http请求服务
        /// </summary>
        /// <typeparam name="TWebApiProxy">应契约接口的WebApiProxy配置文件</typeparam>
        /// <param name="baseUrl">服务器地址，不填时需要在契约接口添加HttpHost标签</param>
        public void AddWebApiProxy<TWebApiProxy>(string baseUrl = "") where TWebApiProxy : WebApiProxy
        {
            if (Activator.CreateInstance(typeof(TWebApiProxy), baseUrl) is WebApiProxy proxy)
                WebApiProxies.Add(proxy);
        }

        public void AddService<TService>(string baseUrl = "")
        {
            if (!typeof(TService).IsInterface)
                throw new ArgumentOutOfRangeException($"{typeof(TService).Name} is not a interface.");

            RegisteredServices.Add(new KeyValuePair<string, Type>(baseUrl, typeof(TService)));
        }
    }
}
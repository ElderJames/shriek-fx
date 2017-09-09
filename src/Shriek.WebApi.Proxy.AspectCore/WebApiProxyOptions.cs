using System;
using System.Collections.Generic;

namespace Shriek.WebApi.Proxy.AspectCore
{
    public class WebApiProxyOptions
    {
        public ICollection<IWebApiProxy> WebApiProxies { get; set; } = new List<IWebApiProxy>();

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
    }
}
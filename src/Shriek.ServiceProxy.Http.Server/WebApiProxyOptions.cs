using System;
using System.Collections.Generic;
using Shriek.ServiceProxy.Abstractions;

namespace Shriek.ServiceProxy.Http.Server
{
    public class WebApiProxyOptions
    {
        public ICollection<IWebApiProxy> WebApiProxies { get; set; } = new List<IWebApiProxy>();

        public void AddWebApiProxy<TWebApiProxy>(string url) where TWebApiProxy : WebApiProxy
        {
            AddWebApiProxy(typeof(TWebApiProxy), url);
        }

        public void AddWebApiProxy(Type webApiProxType, string url)
        {
            if (!typeof(WebApiProxy).IsAssignableFrom(webApiProxType))
                throw new Exception("不是WebApiProxy的子类");

            if (Activator.CreateInstance(webApiProxType, url ?? "") is WebApiProxy proxy)
                WebApiProxies.Add(proxy);
        }
    }
}
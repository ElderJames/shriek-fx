using System;
using System.Collections.Generic;

namespace Shriek.WebApi.Proxy
{
    public class WebApiProxyOptions
    {
        public ICollection<IWebApiProxy> WebApiProxies { get; set; } = new List<IWebApiProxy>();

        public void AddWebApiProxy<TWebApiProxy>(string baseUrl) where TWebApiProxy : WebApiProxy
        {
            if (Activator.CreateInstance(typeof(TWebApiProxy), baseUrl) is WebApiProxy proxy)
                WebApiProxies.Add(proxy);
        }
    }
}
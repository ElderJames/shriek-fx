using System;
using System.Collections.Generic;
using Shriek.ServiceProxy.Abstractions;

namespace Shriek.Mvc
{
    public class WebApiProxyOptions
    {
        public ICollection<IWebApiProxy> WebApiProxies { get; set; } = new List<IWebApiProxy>();

        public void AddWebApiProxy<TWebApiProxy>() where TWebApiProxy : WebApiProxy
        {
            if (Activator.CreateInstance(typeof(TWebApiProxy), "") is WebApiProxy proxy)
                WebApiProxies.Add(proxy);
        }
    }
}
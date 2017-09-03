using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.WebApi.Proxy
{
    public class WebApiProxyOptions
    {
        internal IEnumerable<IWebApiProxyOption> proxyOptions { get; set; }

        public void AddWebApiProxy<TWebApiProxyOption>(TWebApiProxyOption option) where TWebApiProxyOption : IWebApiProxyOption
        {
            proxyOptions.Concat(new IWebApiProxyOption[] { option });
        }
    }
}
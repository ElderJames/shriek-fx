using System.Collections.Generic;
using System.Linq;

namespace Shriek.WebApi.Proxy
{
    public class WebApiProxyOptions
    {
        internal IEnumerable<IWebApiProxyOption> ProxyOptions { get; set; }

        public void AddWebApiProxy<TWebApiProxyOption>(TWebApiProxyOption option) where TWebApiProxyOption : IWebApiProxyOption
        {
            ProxyOptions.Concat(new IWebApiProxyOption[] { option });
        }
    }
}
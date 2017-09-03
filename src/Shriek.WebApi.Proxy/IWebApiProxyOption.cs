using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.WebApi.Proxy
{
    public interface IWebApiProxyOption
    {
        string BaseUrl { get; set; }
    }
}
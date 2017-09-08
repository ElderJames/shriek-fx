using System;
using System.Collections.Generic;

namespace Shriek.WebApi.Proxy.AspectCore
{
    public interface IWebApiProxyOption
    {
        string BaseUrl { get; set; }

        IEnumerable<Type> ServiceTypes { get; }
    }
}
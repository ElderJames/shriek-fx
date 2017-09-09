using System;
using System.Collections.Generic;

namespace Shriek.ServiceProxy.Http
{
    public interface IWebApiProxyOption
    {
        string BaseUrl { get; set; }

        IEnumerable<Type> ServiceTypes { get; }
    }
}
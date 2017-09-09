using System;
using System.Collections.Generic;

namespace Shriek.ServiceProxy.Abstractions
{
    public interface IWebApiProxyOption
    {
        string BaseUrl { get; set; }

        IEnumerable<Type> ServiceTypes { get; }
    }
}
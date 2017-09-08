using System;
using System.Collections.Generic;

namespace Shriek.WebApi.Proxy.AspectCore
{
    public interface IWebApiProxy
    {
        string BaseUrl { get; set; }
    }
}
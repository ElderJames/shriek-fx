using System;
using System.Collections.Generic;

namespace Shriek.WebApi.Proxy
{
    public interface IWebApiProxy
    {
        string BaseUrl { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Shriek.ServiceProxy.Http
{
    public interface IWebApiProxy
    {
        string BaseUrl { get; set; }
    }
}
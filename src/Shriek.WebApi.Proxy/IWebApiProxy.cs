using System;
using System.Collections.Generic;

namespace Shriek.WebApi.Proxy
{
    internal interface IWebApiProxy
    {
        string BaseUrl { get; set; }
    }
}
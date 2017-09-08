using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shriek.WebApi.Proxy.AspectCore
{
    public interface IHttpClient : IDisposable
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage);
    }
}
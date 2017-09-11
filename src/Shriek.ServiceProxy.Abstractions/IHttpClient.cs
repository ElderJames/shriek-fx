using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Abstractions
{
    public interface IHttpClient : IDisposable
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage);
    }
}
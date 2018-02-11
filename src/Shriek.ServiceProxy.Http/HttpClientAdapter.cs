using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Shriek.ServiceProxy.Abstractions;

namespace Shriek.ServiceProxy.Http
{
    public class HttpClientAdapter : IHttpClient
    {
        private static HttpClient httpClient;

        public HttpClientAdapter(HttpClient client)
        {
            httpClient = client;
            httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        public void Dispose()
        {
            //httpClient.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => httpClient.SendAsync(request);
    }
}
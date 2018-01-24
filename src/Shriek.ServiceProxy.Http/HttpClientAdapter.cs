using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Shriek.ServiceProxy.Http
{
    internal class HttpClientAdapter : IHttpClient
    {
        private static HttpClient httpClient;

        public HttpClientAdapter(HttpClient client)
        {
            httpClient = client;
        }

        public void Dispose()
        {
            //httpClient.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => httpClient.SendAsync(request);
    }
}
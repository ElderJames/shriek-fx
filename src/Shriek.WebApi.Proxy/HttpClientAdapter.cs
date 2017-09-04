using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shriek.WebApi.Proxy
{
    internal class HttpClientAdapter : IHttpClient
    {
        private readonly HttpClient httpClient;

        public HttpClientAdapter(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => httpClient.SendAsync(request);
    }
}

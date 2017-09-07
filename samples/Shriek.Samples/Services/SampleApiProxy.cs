using Shriek.WebApi.Proxy;

namespace Shriek.Samples.Services
{
    public class SampleApiProxy : WebApiProxy
    {
        public SampleApiProxy(string baseUrl) : base(baseUrl)
        {
        }
    }
}
using Shriek.WebApi.Proxy.AspectCore;

namespace Shriek.Samples.Services
{
    public class SampleApiProxy : WebApiProxy
    {
        public SampleApiProxy(string baseUrl) : base(baseUrl)
        {
        }
    }
}
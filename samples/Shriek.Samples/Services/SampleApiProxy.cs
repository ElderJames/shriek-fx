using Shriek.ServiceProxy.Http;

namespace Shriek.Samples.Services
{
    public class SampleApiProxy : WebApiProxy
    {
        public SampleApiProxy(string baseUrl) : base(baseUrl)
        {
        }
    }
}
using Shriek.ServiceProxy.Abstractions;

namespace Shriek.Samples.Services
{
    public class SampleApiProxy : WebApiProxy
    {
        public SampleApiProxy(string baseUrl) : base(baseUrl)
        {
        }
    }
}
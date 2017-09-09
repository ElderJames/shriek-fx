using Shriek.ServiceProxy.Abstractions;

namespace Shriek.Samples.WebApiProxy.Contacts
{
    public class SampleApiProxy : ServiceProxy.Abstractions.WebApiProxy
    {
        public SampleApiProxy(string baseUrl) : base(baseUrl)
        {
        }
    }
}
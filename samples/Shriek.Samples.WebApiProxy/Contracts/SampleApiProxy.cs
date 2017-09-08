namespace Shriek.Samples.WebApiProxy.Contacts
{
    public class SampleApiProxy : WebApi.Proxy.AspectCore.WebApiProxy
    {
        public SampleApiProxy(string baseUrl) : base(baseUrl)
        {
        }
    }
}
namespace Shriek.Samples.WebApiProxy.Contacts
{
    public class SampleApiProxy : ServiceProxy.Http.WebApiProxy
    {
        public SampleApiProxy(string baseUrl) : base(baseUrl)
        {
        }
    }
}
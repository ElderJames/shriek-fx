namespace Shriek.Samples.WebApiProxy.Contracts
{
    public class SampleApiProxy : ServiceProxy.Abstractions.WebApiProxy
    {
        public SampleApiProxy(string baseUrl) : base(baseUrl)
        {
        }
    }
}
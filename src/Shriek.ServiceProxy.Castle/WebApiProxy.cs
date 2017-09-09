namespace Shriek.WebApi.Proxy
{
    public abstract class WebApiProxy : IWebApiProxy
    {
        protected WebApiProxy(string baseUrl)
        {
            this.BaseUrl = baseUrl;
        }

        public string BaseUrl { get; set; }
    }
}
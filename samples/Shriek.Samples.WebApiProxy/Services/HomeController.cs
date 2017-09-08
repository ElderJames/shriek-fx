using Microsoft.AspNetCore.Mvc;

namespace Shriek.Samples.WebApiProxy.Services
{
    [Controller]
    public class Home
    {
        public string Index()
        {
            return "ok";
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace Shriek.Samples.WebApiProxy.Services
{
    [Controller]
    public class Home
    {
        [HttpGet, HttpPost]
        public string Index()
        {
            return "ok";
        }
    }
}
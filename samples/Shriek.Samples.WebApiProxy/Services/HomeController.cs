using Microsoft.AspNetCore.Mvc;

namespace Shriek.Samples.WebApiProxy.Services
{
    [Controller]
    public class Home
    {
        [HttpGet("gethome"), HttpPost("posthome")]
        public string Index()
        {
            return "ok";
        }

        public string Hello()
        {
            return "hello";
        }
    }
}
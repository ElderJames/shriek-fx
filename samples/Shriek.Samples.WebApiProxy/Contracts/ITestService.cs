using Shriek.Samples.WebApiProxy.Models;
using Shriek.WebApi.Proxy.AspectCore;

namespace Shriek.Samples.WebApiProxy.Contacts
{
    [Route("route")]
    public interface ITestService
    {
        [HttpGet("test/{id:int}")]
        Todo Test(int id);
    }
}
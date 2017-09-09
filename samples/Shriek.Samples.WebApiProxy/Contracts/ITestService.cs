using Shriek.Samples.WebApiProxy.Models;
using Shriek.ServiceProxy.Http;

namespace Shriek.Samples.WebApiProxy.Contacts
{
    [Route("route")]
    public interface ITestService
    {
        [HttpGet("test/{id:int}")]
        Todo Test(int id);
    }
}
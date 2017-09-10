using Shriek.Samples.WebApiProxy.Models;
using Shriek.ServiceProxy.Http;

namespace Shriek.Samples.WebApiProxy.Contracts
{
    [Route("route")]
    public interface ITestService
    {
        [HttpGet("test/{id:int}")]
        Todo Test(int id);
    }
}
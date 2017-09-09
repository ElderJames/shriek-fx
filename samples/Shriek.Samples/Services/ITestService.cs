using Shriek.Samples.Models;
using Shriek.ServiceProxy.Http;

namespace Shriek.Samples.Services
{
    [Route("route")]
    public interface ITestService
    {
        [HttpGet("test/{id:int}")]
        Todo Test(int id);
    }
}
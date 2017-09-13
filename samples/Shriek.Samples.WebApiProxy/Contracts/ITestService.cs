using Shriek.Samples.WebApiProxy.Models;
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Http;

namespace Shriek.Samples.WebApiProxy.Contracts
{
    [HttpHost("http://localhost:8081")]
    [Route("route")]
    public interface ITestService
    {
        [HttpGet("test/{id:int}")]
        Todo Test(int id);
    }
}
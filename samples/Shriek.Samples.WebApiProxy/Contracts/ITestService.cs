using Shriek.Samples.WebApiProxy.Models;
using Shriek.ServiceProxy.Abstractions.Attributes;
using Shriek.ServiceProxy.Http.ActionAttributes;

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
using Shriek.ServiceProxy.Abstractions;
using Shriek.ServiceProxy.Http;

namespace Shriek.Samples.Services
{
    [Route("test")]
    public interface ITestService
    {
        [HttpGet("{name}")]
        string Test(string name);
    }
}
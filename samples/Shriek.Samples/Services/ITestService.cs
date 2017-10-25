using System.Threading.Tasks;
using Shriek.ServiceProxy.Abstractions.Attributes;
using Shriek.ServiceProxy.Http.ActionAttributes;

namespace Shriek.Samples.Services
{
    [Route("test")]
    public interface ITestService
    {
        [HttpGet("{name}")]
        Task<string> Test(string name);
    }
}
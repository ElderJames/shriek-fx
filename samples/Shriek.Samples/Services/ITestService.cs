using Shriek.ServiceProxy.Abstractions.Attributes;
using Shriek.ServiceProxy.Http.ActionAttributes;
using System.Threading.Tasks;

namespace Shriek.Samples.Services
{
    [Route("test")]
    public interface ITestService
    {
        [HttpGet("{name}")]
        Task<string> Test(string name);
    }
}
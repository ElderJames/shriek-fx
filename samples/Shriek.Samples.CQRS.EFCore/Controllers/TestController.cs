using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shriek.Samples.Services;

namespace Shriek.Samples.CQRS.EFCore.Controllers
{
    public class TestController : Controller, ITestService
    {
        public Task<string> Test(string name)
        {
            return Task.FromResult("your name is " + name);
        }
    }
}
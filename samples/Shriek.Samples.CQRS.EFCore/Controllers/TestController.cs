using Microsoft.AspNetCore.Mvc;
using Shriek.Samples.Services;
using System.Threading.Tasks;

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
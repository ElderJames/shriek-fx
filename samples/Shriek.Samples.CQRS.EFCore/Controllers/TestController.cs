using Microsoft.AspNetCore.Mvc;
using Shriek.Samples.Models;
using Shriek.Samples.Services;

namespace Shriek.Samples.CQRS.EFCore.Controllers
{
    public class TestController : Controller, ITestService
    {
        public string Test(string name)
        {
            return "your name is " + name;
        }
    }
}
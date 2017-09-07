using Microsoft.AspNetCore.Mvc;
using Shriek.Samples.Models;
using Shriek.Samples.Services;

namespace Shriek.Samples.CQRS.EFCore.Controllers
{
    public class TestController : Controller, ITestService
    {
        public Todo Test(int id)
        {
            return new Todo() { Name = id.ToString() };
        }
    }
}
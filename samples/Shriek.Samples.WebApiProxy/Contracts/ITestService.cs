using Microsoft.AspNetCore.Mvc;
using Shriek.Samples.WebApiProxy.Models;

namespace Shriek.Samples.WebApiProxy.Contacts
{
    [ServiceProxy.Http.Route("route")]
    public interface ITestService
    {
        [HttpGet("test/{id:int}")]
        Todo Test(int id);
    }
}
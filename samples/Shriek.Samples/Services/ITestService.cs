using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Samples.Models;
using Shriek.WebApi.Proxy;

namespace Shriek.Samples.Services
{
    [Route("route")]
    public interface ITestService
    {
        [HttpGet("test/{id:int}")]
        Todo Test(int id);
    }
}
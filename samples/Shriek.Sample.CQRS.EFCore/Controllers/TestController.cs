using Microsoft.AspNetCore.Mvc;

namespace Shriek.Samples.CQRS.EFCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Test")]
    public class TestController : Controller
    {
        [HttpGet("{name}")]
        public string GetName(string name)
        {
            return name;
        }

        [HttpGet("{id:int}")]
        public int GetName(int id)
        {
            return id;
        }
    }
}
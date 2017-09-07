using Shriek.Samples.Models;
using Shriek.WebApi.Proxy;

namespace Shriek.Samples.Services
{
    [Route("api")]
    public interface ITodoService
    {
        [HttpPost("todo")]
        Todo Create([JsonContent] Todo todo);

        [Route("route/{id:int}")]
        [HttpGet("route2/{id:int}")]
        Todo Get(int id);

        [HttpGet("{name}")]
        Todo Get(string name);
    }
}
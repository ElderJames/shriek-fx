using Shriek.Samples.Models;
using Shriek.WebApi.Proxy;

namespace Shriek.Samples.Services
{
    [Route("api")]
    public interface ITodoService
    {
        [HttpPost("todo")]
        bool Create([JsonContent] Todo todo);

        [HttpGet("{id:int}")]
        Todo Get(int id);

        [HttpGet("{name}")]
        Todo Get(string name);
    }
}
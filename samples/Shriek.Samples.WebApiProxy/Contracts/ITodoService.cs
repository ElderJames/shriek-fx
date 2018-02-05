using System.ComponentModel;
using Shriek.Samples.WebApiProxy.Models;
using Shriek.ServiceProxy.Abstractions.Attributes;
using Shriek.ServiceProxy.Http.ActionAttributes;
using System.Threading.Tasks;

namespace Shriek.Samples.WebApiProxy.Contracts
{
    [HttpHost("http://localhost:8080")]
    [Route("api")]
    public interface ITodoService
    {
        //[HttpPost("todo")]
        Task<Todo> Create(Todo todo);

        //[Route("route/{id:int}")]
        //[HttpGet("route2/{id:int}")]
        Task<Todo> Get(int id);

        [HttpGet("{name}")]
        Task<Todo> Get(string name);

        //[HttpGet("types")]
        Type[] GetTypes(Type[] types, string name, int age);
    }

    public enum Type : ulong
    {
        [Description("描述：睡觉")]
        睡觉 = 1 << 20,

        工作 = 1 << 21,

        [Description("描述：起床")]
        起床 = 1 << 22,
    }
}
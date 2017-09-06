using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Shriek.Samples.Models;
using Shriek.WebApi.Proxy;

namespace Shriek.Samples.Services
{
    [Route("api")]
    public interface ITodoService
    {
        [HttpPost("todo")]
        bool Create([JsonContent] Todo todo);

        [HttpGet("{id}")]
        Todo Get(int id);
    }
}

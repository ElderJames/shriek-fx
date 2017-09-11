using Microsoft.AspNetCore.Mvc;
using Shriek.Samples.Models;
using Shriek.Samples.Services;
using System;
using System.Threading.Tasks;

namespace Shriek.Samples.CQRS.EFCore.Controllers
{
    public class TodoServiceController : Controller, ITodoService
    {
        public Task<Todo> Create(Todo todo)
        {
            return Task.FromResult(todo);
        }

        public Task<Todo> Get(int id)
        {
            return Task.FromResult(new Todo { AggregateId = Guid.NewGuid(), Name = "起床", Desception = "上班", FinishTime = DateTime.Now });
        }

        public Task<Todo> Get(string name)
        {
            return Task.FromResult(new Todo { AggregateId = Guid.NewGuid(), Name = name, Desception = "上班", FinishTime = DateTime.Now });
        }
    }
}
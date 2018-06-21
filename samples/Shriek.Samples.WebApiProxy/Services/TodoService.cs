using Shriek.Samples.WebApiProxy.Contracts;
using Shriek.Samples.WebApiProxy.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Type = Shriek.Samples.WebApiProxy.Contracts.Type;

namespace Shriek.Samples.WebApiProxy.Services
{
    public class TodoService : ITodoService
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

        public Type[] GetTypes(Type[] types, string name, int age)
        {
            return types;
        }
    }
}
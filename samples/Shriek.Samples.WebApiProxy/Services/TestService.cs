using Shriek.Samples.WebApiProxy.Contracts;
using Shriek.Samples.WebApiProxy.Models;
using System;

namespace Shriek.Samples.WebApiProxy.Services
{
    public class TestService : ITestService
    {
        private ITodoService todoService;

        public TestService(ITodoService todoService)
        {
            this.todoService = todoService;
        }

        public Todo Test(int id)
        {
            return todoService.Create(new Todo()
            {
                AggregateId = Guid.NewGuid(),
                Desception = "从服务内调用调用",
                Name = "form service",
                FinishTime = DateTime.Now
            }).Result;
        }
    }
}
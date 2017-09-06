using System;
using Microsoft.AspNetCore.Mvc;
using Shriek.Commands;
using Shriek.Samples.Commands;
using Shriek.Samples.Models;
using Shriek.Samples.Services;
using Shriek.WebApi.Proxy;

namespace Shriek.Samples.CQRS.EFCore.Controllers
{
    public class TodoServiceController : Controller, ITodoService
    {

        public bool Create(Todo todo)
        {
            return true;
        }

        public Todo Get(int id)
        {
            return new Todo { AggregateId = Guid.NewGuid(), Name = "起床", Desception = "上班", FinishTime = DateTime.Now };
        }
    }
}
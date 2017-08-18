using Newtonsoft.Json;
using System;
using Microsoft.Extensions.DependencyInjection;
using Shriek.Commands;
using Shriek.ConfigCenter.Domain.Aggregates;
using Shriek.ConfigCenter.Domain.Commands;
using Shriek.Storage;

namespace Shriek.ConfigCenter.Tests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddShriek();

            var container = services.BuildServiceProvider();

            var bus = container.GetService<ICommandBus>();
            var repository = container.GetService<IRepository<ConfigItemAggregateRoot>>();

            var id = Guid.NewGuid();

            bus.Send(new CreateConfigItemCommand(id)
            {
                Name = "ysj",
                Value = "very good"
            });

            var root = repository.GetById(id);
            Console.WriteLine(JsonConvert.SerializeObject(root));

            bus.Send(new ChangeConfigItemCommand(id)
            {
                Name = "Cho",
                Value = "Beautiful!"
            });
            root = repository.GetById(id);
            Console.WriteLine(JsonConvert.SerializeObject(root));
            Console.WriteLine("command handled");

            Console.ReadKey();
        }
    }
}
using System.Diagnostics;
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
            var eventStorage = container.GetService<IEventStorage>();

            var id = Guid.NewGuid();

            bus.Send(new CreateConfigItemCommand(id)
            {
                Name = "ysj",
                Value = "very good"
            });

            Debug.WriteLine($"{nameof(CreateConfigItemCommand)} sended!");

            bus.Send(new ChangeConfigItemCommand(id)
            {
                Name = "Cho",
                Value = "Beautiful!"
            });

            Debug.WriteLine($"{nameof(ChangeConfigItemCommand)} sended!");

            bus.Send(new ChangeConfigItemCommand(id)
            {
                Name = "Ron",
                Value = "hansome!"
            });

            bus.Send(new ChangeConfigItemCommand(id)
            {
                Name = "Luna",
                Value = "Beautiful!"
            });

            bus.Send(new ChangeConfigItemCommand(id)
            {
                Name = "Albus",
                Value = "great!"
            });

            bus.Send(new ChangeConfigItemCommand(id)
            {
                Name = "throw exception",
                Value = "throw exception"
            });

            Console.ReadKey();
        }
    }
}
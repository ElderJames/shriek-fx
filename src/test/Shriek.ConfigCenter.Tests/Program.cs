using System.Diagnostics;
using System;
using Microsoft.Extensions.DependencyInjection;
using Shriek.Commands;
using Shriek.Console.Commands;
using Shriek.Storage;

namespace Shriek.Console.Tests
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

            System.Console.WriteLine($"{nameof(CreateConfigItemCommand)} sended!");

            bus.Send(new ChangeConfigItemCommand(id)
            {
                Name = "Cho",
                Value = "Beautiful!"
            });

            System.Console.WriteLine($"{nameof(ChangeConfigItemCommand)} sended!");

            bus.Send(new ChangeConfigItemCommand(id)
            {
                Name = "Ron",
                Value = "hansome!"
            });
            System.Console.WriteLine($"{nameof(ChangeConfigItemCommand)} sended!");

            bus.Send(new ChangeConfigItemCommand(id)
            {
                Name = "Luna",
                Value = "Beautiful!"
            });
            System.Console.WriteLine($"{nameof(ChangeConfigItemCommand)} sended!");

            bus.Send(new ChangeConfigItemCommand(id)
            {
                Name = "Albus",
                Value = "great!"
            });
            System.Console.WriteLine($"{nameof(ChangeConfigItemCommand)} sended!");

            bus.Send(new ChangeConfigItemCommand(id)
            {
                Name = "throw exception",
                Value = "throw exception"
            });
            System.Console.WriteLine($"{nameof(ChangeConfigItemCommand)} sended!");

            System.Console.ReadKey();
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using Shriek.Samples.Commands;
using System;
using Microsoft.Extensions.Caching.Redis;
using Shriek.Commands;
using Shriek.EventStorage.Redis;

namespace Shriek.Samples.EventStorage.Redis
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddShriek(option => option.UseRedisEventStorage(optinos =>
            {
                optinos.Configuration = "localhost:32768";
                optinos.InstanceName = "shriek.events";
            }));

            var container = services.BuildServiceProvider();

            var bus = container.GetService<ICommandBus>();

            var id = Guid.NewGuid();
            bus.Send(new CreateTodoCommand(id)
            {
                Name = "get up",
                Desception = "good day",
                FinishTime = DateTime.Now.AddDays(1)
            });

            Console.WriteLine($"{nameof(CreateTodoCommand)} sended!");

            bus.Send(new ChangeTodoCommand(id)
            {
                Name = "eat breakfast",
                Desception = "yummy!",
                FinishTime = DateTime.Now.AddDays(1)
            });

            Console.WriteLine($"{nameof(ChangeTodoCommand)} sended!");

            bus.Send(new ChangeTodoCommand(id)
            {
                Name = "go to work",
                Desception = "fighting!",
                FinishTime = DateTime.Now.AddDays(1)
            });
            Console.WriteLine($"{nameof(ChangeTodoCommand)} sended!");

            bus.Send(new ChangeTodoCommand(id)
            {
                Name = "call boss",
                Desception = "haha!",
                FinishTime = DateTime.Now.AddDays(1)
            });
            Console.WriteLine($"{nameof(ChangeTodoCommand)} sended!");

            bus.Send(new ChangeTodoCommand(id)
            {
                Name = "coding",
                Desception = "great!",
                FinishTime = DateTime.Now.AddDays(1)
            });
            Console.WriteLine($"{nameof(ChangeTodoCommand)} sended!");

            try
            {
                bus.Send(new ChangeTodoCommand(id)
                {
                    Name = "drive car",
                    Desception = "be careful!",
                    FinishTime = DateTime.Now.AddDays(-1)
                });
            }
            catch (Exception ex)
            {
            }
            Console.WriteLine($"{nameof(ChangeTodoCommand)} sended!");

            Console.ReadKey();
        }
    }
}
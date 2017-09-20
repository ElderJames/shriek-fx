using Microsoft.Extensions.DependencyInjection;
using Shriek.Commands;
using Shriek.Samples.Commands;
using Shriek.Storage;
using System;

namespace Shriek.Samples.InProcess
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

            bus.Send(new CreateTodoCommand(id)
            {
                Name = "1. get up",
                Desception = "good day",
                FinishTime = DateTime.Now.AddDays(1)
            });

            Console.WriteLine($"{nameof(CreateTodoCommand)} sended!");

            //bus.Send(new ChangeTodoCommand(id)
            //{
            //    Name = "2. eat breakfast",
            //    Desception = "yummy!",
            //    FinishTime = DateTime.Now.AddDays(1)
            //});

            //Console.WriteLine($"{nameof(ChangeTodoCommand)} sended!");

            //bus.Send(new ChangeTodoCommand(id)
            //{
            //    Name = "3. go to work",
            //    Desception = "fighting!",
            //    FinishTime = DateTime.Now.AddDays(1)
            //});
            //Console.WriteLine($"{nameof(ChangeTodoCommand)} sended!");

            //bus.Send(new ChangeTodoCommand(id)
            //{
            //    Name = "4. call boss",
            //    Desception = "haha!",
            //    FinishTime = DateTime.Now.AddDays(1)
            //});
            //Console.WriteLine($"{nameof(ChangeTodoCommand)} sended!");

            //bus.Send(new ChangeTodoCommand(id)
            //{
            //    Name = "5. coding",
            //    Desception = "great!",
            //    FinishTime = DateTime.Now.AddDays(1)
            //});
            //Console.WriteLine($"{nameof(ChangeTodoCommand)} sended!");

            //bus.Send(new ChangeTodoCommand(id)
            //{
            //    Name = "6. drive car",
            //    Desception = "be careful!",
            //    FinishTime = DateTime.Now.AddDays(-1)
            //});
            //Console.WriteLine($"{nameof(ChangeTodoCommand)} sended!");

            var i = 0;

            while (true)
            {
                Console.ReadKey();

                try
                {
                    bus.Send(new ChangeTodoCommand(id)
                    {
                        Name = $"command no.{++i}",
                        Desception = "be careful!",
                        FinishTime = DateTime.Now.AddDays(1)
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception:" + ex.Message);
                }
                Console.WriteLine($"{i} {nameof(ChangeTodoCommand)} sended!");
            }

            Console.ReadKey();
        }
    }
}
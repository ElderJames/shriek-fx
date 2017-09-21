using Microsoft.Extensions.DependencyInjection;
using Shriek.Commands;
using Shriek.Samples.Commands;
using Shriek.Storage;
using System;
using Shriek.Samples.InProcess.Commands;

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

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

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
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.D1)
                {
                    try
                    {
                        bus.Send(new SampleCommand(id1)
                        {
                            No = 1,
                            Delay = TimeSpan.FromMilliseconds(5000)
                        });

                        Console.WriteLine($"id-1: command {i} sended!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception:" + ex.Message);
                    }
                }
                else if (key.Key == ConsoleKey.D2)
                {
                    try
                    {
                        bus.Send(new SampleCommand(id2)
                        {
                            No = 2,
                            Delay = TimeSpan.FromMilliseconds(2000)
                        });

                        Console.WriteLine($"id-2: command {i} sended!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception:" + ex.Message);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
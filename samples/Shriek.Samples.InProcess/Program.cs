using Shriek.Samples.Commands;
using Microsoft.Extensions.DependencyInjection;
using Shriek.Commands;
using Shriek.Samples.InProcess.Commands;
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

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var i = 0;
            var j = 0;

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

                        Console.WriteLine($"\tid-1: \tcommand {i++} sended!");
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

                        Console.WriteLine($"\tid-2: \tcommand {j++} sended!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception:" + ex.Message);
                    }
                }
                else if (key.Key == ConsoleKey.D3)
                {
                    try
                    {
                        bus.Send(new CreateTodoCommand(Guid.NewGuid()) { Desception = "hello other assembly!", FinishTime = DateTime.Now.AddDays(-1) });
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
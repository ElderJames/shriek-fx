using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Shriek.Commands;
using Shriek.EventStorage.Dapper;
using Shriek.Samples.Dapper.Commands;
using Shriek.Storage;
using System;

namespace Shriek.Samples.Dapper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot Configuration = builder.Build();

            var services = new ServiceCollection();

            var connStr = Configuration.GetConnectionString("eventStore");
            services.AddShriek(option => option.UseDapperEventStorage(options => options.DbConnection = new NpgsqlConnection(connStr)));

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
                else
                {
                    break;
                }
            }
        }
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shriek.Commands;
using Shriek.EventStorage.InfluxDB;
using Shriek.Samples.Commands;
using System;

namespace Shriek.Samples.EventStorage.InfluxDB
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var services = new ServiceCollection();

            services.AddShriek(option => option.UseInfluxDbEventStorage(options =>
                  {
                      options.Host = configuration.GetSection("InfluxDBConnection:Host").Value;
                      options.Password = configuration.GetSection("InfluxDBConnection:Password").Value;
                      options.UserName = configuration.GetSection("InfluxDBConnection:UserName").Value;
                      options.DatabaseName = configuration.GetSection("InfluxDBConnection:DatabaseName").Value;
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

            bus.Send(new ChangeTodoCommand(id)
            {
                Name = "drive car",
                Desception = "be careful!",
                FinishTime = DateTime.Now.AddDays(1)
            });
            Console.WriteLine($"{nameof(ChangeTodoCommand)} sended!");

            Console.ReadKey();
        }
    }
}
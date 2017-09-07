using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shriek.Samples.CQRS.EFCore;
using Shriek.Samples.Services;
using Shriek.WebApi.Proxy;

namespace Shriek.Samples.WebApiProxy
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:8080")
                .Build()
                .RunAsync();

            Console.ReadKey();

            var services = new ServiceCollection();
            services.AddWebApiProxy(option =>
            {
                option.AddWebApiProxy<SampleApiProxy>("http://localhost:8080"); ;
            });

            var provider = services.BuildServiceProvider();

            var todoService = provider.GetService<ITodoService>();
            var testService = provider.GetService<ITestService>();

            var result = todoService.Get(1).Result;
            Console.WriteLine(JsonConvert.SerializeObject(result));
            Console.ReadKey();

            var result2 = testService.Test(11);
            Console.WriteLine(JsonConvert.SerializeObject(result2));
            Console.ReadKey();
        }
    }
}
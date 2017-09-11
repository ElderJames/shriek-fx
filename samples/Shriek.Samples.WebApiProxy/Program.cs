using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shriek.Samples.WebApiProxy.Contracts;
using Shriek.ServiceProxy.Http;
using Shriek.ServiceProxy.Http.Server;
using System;

namespace Shriek.Samples.WebApiProxy
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseShriekStartup("http://localhost:8080")
                .Build().Start();

            var provider = new ServiceCollection()
                .AddShriek()
                .AddWebApiProxy(opt => opt.AddWebApiProxy<SampleApiProxy>())
                .Services
                .BuildServiceProvider();

            var todoService = provider.GetService<ITodoService>();
            var testService = provider.GetService<ITestService>();

            Console.ReadKey();

            var result = todoService.Get(1).Result;
            Console.WriteLine(JsonConvert.SerializeObject(result));

            var result2 = testService.Test(11);
            Console.WriteLine(JsonConvert.SerializeObject(result2));

            Console.ReadKey();
        }
    }
}
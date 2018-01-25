using Microsoft.Extensions.DependencyInjection;
using Shriek.Samples.WebApiProxy.Contracts;
using System;
using System.Net;
using System.Threading.Tasks;
using Shriek.ServiceProxy.Socket.Core;
using Shriek.ServiceProxy.Socket.Fast;
using Shriek.ServiceProxy.Socket.Server;

namespace Shriek.Samples.WebApiProxy
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //var listener = new TcpListener();
            //listener.Use<FastMiddleware>();
            //listener.Start(1212);

            var services = new ServiceCollection().AddShriek().AddSocketServer(option =>
            {
                option.AddService<ISimpleInterface>();
            });

            Console.ReadKey();

            var client = new FastTcpClient();
            client.Connect(IPAddress.Loopback, 1212);
            var users = client.InvokeApi<string>("About", "admin").GetResult();
            Console.Write(users);

            Console.ReadKey();
            //    new WebHostBuilder()
            //        .UseKestrel()
            //        .UseUrls("http://*:8080", "http://*:8081")
            //        .ConfigureServices(services =>
            //        {
            //            services.AddMvcCore()
            //                .AddJsonFormatters()
            //                .AddWebApiProxyServer(opt =>
            //                    {
            //                        opt.AddWebApiProxy<SampleApiProxy>();
            //                        opt.AddWebApiProxy<Samples.Services.SampleApiProxy>();
            //                        opt.AddService<ISimpleInterface>();
            //                    });

            //            //服务里注册代理客户端
            //            services.AddWebApiProxy(opt =>
            //            {
            //                opt.AddWebApiProxy<SampleApiProxy>("http://localhost:8081");
            //            });
            //        })
            //        .Configure(app =>
            //        {
            //            app.Use(async (context, next) =>
            //            {
            //                try
            //                {
            //                    await next();
            //                }
            //                catch (Exception ex)
            //                {
            //                    throw;
            //                }
            //            });
            //            app.UseMvc();
            //        })
            //        .Build()
            //        .Start();

            //    var provider = new ServiceCollection()
            //        .AddWebApiProxy(opt =>
            //        {
            //            opt.AddWebApiProxy<SampleApiProxy>("http://localhost:8081");
            //            opt.AddWebApiProxy<Samples.Services.SampleApiProxy>("http://localhost:8080");
            //            opt.AddService<ISimpleInterface>("http://localhost:8080");
            //        })
            //        .BuildServiceProvider();

            //    var todoService = provider.GetService<ITodoService>();
            //    var testService = provider.GetService<ITestService>();
            //    var sampleTestService = provider.GetService<Samples.Services.ITestService>();
            //    var tcpService = provider.GetService<ISimpleInterface>();

            //    Console.ReadKey();

            //    var createRsult = todoService.Create(new Todo() { Name = "james" }).Result;
            //    Console.WriteLine(JsonConvert.SerializeObject(createRsult));

            //    var result = todoService.Get(1).Result;
            //    Console.WriteLine(JsonConvert.SerializeObject(result));

            //    result = todoService.Get(2).Result;
            //    Console.WriteLine(JsonConvert.SerializeObject(result));

            //    var typeResult = todoService.GetTypes(new[] { Contracts.Type.起床, Contracts.Type.睡觉 }, "james", 10);
            //    Console.WriteLine(JsonConvert.SerializeObject(typeResult));

            //    //这个调用服务，服务内注入了一个代理客户端调用另一个服务
            //    var result2 = testService.Test(11);
            //    Console.WriteLine(JsonConvert.SerializeObject(result2));

            //    var result3 = sampleTestService.Test("elderjames").Result;
            //    Console.WriteLine(JsonConvert.SerializeObject(result3));

            //    var result4 = tcpService.Test("hahaha").Result;
            //    Console.WriteLine(JsonConvert.SerializeObject(result4));

            //    Console.ReadKey();
        }
    }

    public class FastService : FastApiService
    {
        [Api]
        public UserInfo[] UserInfo(string name)
        {
            return new UserInfo[0];
        }

        [Api]
        public async Task<string> AboutAsync(string name)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return "Fast->" + name;
        }
    }

    public class UserInfo
    {
        public string Name { get; set; }
    }
}
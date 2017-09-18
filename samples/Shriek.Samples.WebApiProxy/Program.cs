using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shriek.Samples.WebApiProxy.Contracts;
using Shriek.ServiceProxy.Http;
using Shriek.ServiceProxy.Http.Server;
using System;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Shriek.Samples.WebApiProxy.Services;
using Shriek.ServiceProxy.Tcp;
using Shriek.ServiceProxy.Tcp.Communication;
using Shriek.ServiceProxy.Tcp.Server;

namespace Shriek.Samples.WebApiProxy
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //new WebHostBuilder()
            //.UseKestrel()
            //.UseShriekStartup("http://localhost:8080")
            //.Build()
            //.Start();

            new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://*:8080", "http://*:8081")
                .ConfigureServices(services =>
                {
                    services.AddMvcCore()
                    .AddJsonFormatters()
                    .AddWebApiProxy();

                    //服务里注册代理客户端
                    services.AddShriek()
                        .AddWebApiProxy(opt =>
                        {
                            opt.AddWebApiProxy<SampleApiProxy>("http://localhost:8081");
                            opt.AddWebApiProxy<Samples.Services.SampleApiProxy>("http://localhost:8080");
                        });
                })
                .Configure(app => app.UseMvc())
                .Build()
                .Start();

            var config = new ChannelConfig
            {
                ReceiveTimeout = TimeSpan.FromSeconds(20),
                SendTimeout = TimeSpan.FromSeconds(20)
            };
            var host = new ServiceHost<TcpTestService>(9091);

            host.AddContract<ITcpTestService>(config);

            host.ServiceInstantiated += s =>
            {
                //construct the created instance
            };

            host.Open().Wait();

            var provider = new ServiceCollection()
                .AddScoped<ITcpTestService, TcpTestService>()
                .AddShriek()
                .AddWebApiProxy(opt =>
                {
                    opt.AddWebApiProxy<SampleApiProxy>("http://localhost:8081");
                    opt.AddWebApiProxy<Samples.Services.SampleApiProxy>("http://localhost:8080");
                })
                .AddTcpServiceProxy(opt =>
                {
                    opt.AddTcpProxy<ITcpTestService>("localhost", 9091, config);
                })
                .Services
                .BuildAspectCoreServiceProvider();

            var todoService = provider.GetService<ITodoService>();
            var testService = provider.GetService<ITestService>();
            var sampleTestService = provider.GetService<Samples.Services.ITestService>();
            var tcpService = provider.GetService<ITcpTestService>();

            Console.ReadKey();

            var result = todoService.Get(1).Result;
            Console.WriteLine(JsonConvert.SerializeObject(result));

            result = todoService.Get(2).Result;
            Console.WriteLine(JsonConvert.SerializeObject(result));

            //这个调用服务，服务内注入了一个代理客户端调用另一个服务
            var result2 = testService.Test(11);
            Console.WriteLine(JsonConvert.SerializeObject(result2));

            var result3 = sampleTestService.Test("elderjames");
            Console.WriteLine(JsonConvert.SerializeObject(result3));

            Console.WriteLine("press any key to tcp testing...");
            Console.ReadKey();

            var result4 = tcpService.Test("hahaha").Result;
            Console.WriteLine(JsonConvert.SerializeObject(result4));

            Console.ReadKey();
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using Shriek.Samples.WebApiProxy.Contracts;
using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Shriek.Samples.WebApiProxy.Models;
using Shriek.ServiceProxy.Http;
using Shriek.ServiceProxy.Http.Server;
using Shriek.ServiceProxy.Socket;
using Shriek.ServiceProxy.Socket.Server;
using Swashbuckle.AspNetCore.Swagger;

namespace Shriek.Samples.WebApiProxy
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://*:8080", "http://*:8081")
                .ConfigureServices(services =>
                {
                    //tcp服务端
                    services.AddSocketServer(options =>
                    {
                        options.EndPoint = new IPEndPoint(IPAddress.Loopback, 1212);
                        options.AddService<ISimpleInterface>();
                    });

                    //tcp客户端
                    services.AddSocketProxy(options =>
                    {
                        options.EndPoint = new IPEndPoint(IPAddress.Loopback, 1212);
                        options.AddService<ISimpleInterface>();
                    });

                    services.AddMvcCore().AddJsonFormatters().AddApiExplorer();

                    //services.AddButterflyForShriek(opt =>
                    //{
                    //    opt.CollectorUrl = "http://localhost:9618";
                    //    opt.Service = "shriek.sample.backend";
                    //});

                    services.AddWebApiProxyServer(opt =>
                    {
                        opt.AddWebApiProxy<SampleApiProxy>();
                        opt.AddWebApiProxy<Samples.Services.SampleApiProxy>();
                        opt.AddService<Samples.Services.ITestService>();
                    });

                    //服务里注册代理客户端
                    services.AddWebApiProxy(opt => { opt.AddWebApiProxy<SampleApiProxy>("http://localhost:8081"); });

                    services.AddRouteAnalyzer();

                    services.AddSwaggerGen(opt =>
                    {
                        opt.SwaggerDoc("v1", new Info { Title = "Shriek sample API", Version = "v1" });
                        opt.CustomSchemaIds(x => x.FullName);
                    });
                })
                .Configure(app =>
                {
                    app.Use(async (context, next) =>
                    {
                        try
                        {
                            await next();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    });

                    var virtualPath = "";

                    app.UseSwagger(c =>
                    {
                        c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.BasePath = virtualPath);
                    });

                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint(virtualPath + "/swagger/v1/swagger.json", "Shriek sample API");
                        c.RoutePrefix = string.Empty;
                    });

                    app.UseMvc(routes =>
                    {
                        routes.MapRoute(
                            name: "default",
                            template: "{controller=Home}/{action=Index}/{id?}");

                        routes.MapRouteAnalyzer("/routes"); // Add
                    });

                    //var routeAnalyzer = app.ApplicationServices.GetService<IRouteAnalyzer>();
                    //foreach (var info in routeAnalyzer.GetAllRouteInformations())
                    //{
                    //    Console.WriteLine(info);
                    //}
                })
                .Build()
                .Start();

            var service = new ServiceCollection();

            //service.AddButterflyForShriekConsole(opt =>
            //{
            //    opt.CollectorUrl = "http://localhost:9618";
            //    opt.Service = "shriek.sample.console";
            //});

            service.AddWebApiProxy(opt =>
            {
                opt.AddWebApiProxy<SampleApiProxy>("http://localhost:8081");
                opt.AddWebApiProxy<Samples.Services.SampleApiProxy>("http://localhost:8080");
                opt.AddService<Samples.Services.ITestService>("http://localhost:8080");
            })
            .AddSocketProxy(options =>
            {
                options.ProxyHost = "localhost:1212";
                options.AddService<ISimpleInterface>();
            });

            var provider = service.BuildServiceProvider();

            var todoService = provider.GetService<ITodoService>();
            var testService = provider.GetService<ITestService>();
            var sampleTestService = provider.GetService<Samples.Services.ITestService>();
            var tcpService = provider.GetService<ISimpleInterface>();
            Console.ReadKey();

            var createRsult = todoService.Create(new Todo() { Name = "james", AggregateId = Guid.NewGuid(), Desception = "hhhh", FinishTime = DateTime.Now }).Result;
            Console.WriteLine(JsonConvert.SerializeObject(createRsult));

            var result = todoService.Get(1).Result;
            Console.WriteLine(JsonConvert.SerializeObject(result));

            result = todoService.Get(2).Result;
            Console.WriteLine(JsonConvert.SerializeObject(result));

            var typeResult = todoService.GetTypes(new[] { Contracts.Type.起床, Contracts.Type.工作, Contracts.Type.睡觉 }, "james", 10);
            Console.WriteLine(JsonConvert.SerializeObject(typeResult.Select(x => x.GetDescription())));

            //这个调用服务，服务内注入了一个代理客户端调用另一个服务
            var result2 = testService.Test(11);
            Console.WriteLine(JsonConvert.SerializeObject(result2));

            var result3 = sampleTestService.Test("elderjames").Result;
            Console.WriteLine(JsonConvert.SerializeObject(result3));

            var result4 = tcpService.Test("hahaha").Result;
            Console.WriteLine(JsonConvert.SerializeObject(result4));

            Console.ReadKey();
        }
    }
}
using System;
using System.Threading.Tasks;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shriek.ServiceProxy.Tcp;
using Shriek.ServiceProxy.Tcp.Attributes;
using Shriek.ServiceProxy.Tcp.Communication;
using Shriek.ServiceProxy.Tcp.Server;

namespace Shriek.Test.WebApiProxy
{
    [TestClass]
    public class TcpTest
    {
        [TestMethod]
        public async Task TestGetClient()
        {
            var config = new ChannelConfig
            {
                ReceiveTimeout = TimeSpan.FromSeconds(20),
                SendTimeout = TimeSpan.FromSeconds(20)
            };

            var host = new ServiceHost<TestService>(9091);

            host.AddContract<ITestService>(config);

            host.ServiceInstantiated += s =>
            {
                //construct the created instance
            };

            await host.Open();

            var services = new ServiceCollection();

            //  services.AddScoped<ITestService, TestService>();

            services.AddShriek(option =>
            {
                option.UseTcpServiceProxy(options =>
                {
                    options.AddTcpProxy<ITestService>("localhost", 9091, config);
                });
            });

            var provider = services.BuildAspectCoreServiceProvider();

            var client = provider.GetService<ITestService>();

            Assert.IsNotNull(client);

            var result = await client.GetName("elderjaems");

            Assert.AreEqual("elderjaems", result);
        }
    }

    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        Task<string> GetName(string name);
    }

    public class TestService : ITestService
    {
        public Task<string> GetName(string name)
        {
            return Task.FromResult(name);
        }
    }
}
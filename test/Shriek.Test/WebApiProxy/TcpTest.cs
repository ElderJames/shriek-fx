using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shriek.ServiceProxy.Tcp;
using TcpServiceCore.Attributes;
using TcpServiceCore.Client;
using TcpServiceCore.Communication;
using TcpServiceCore.Server;

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

            services.AddScoped<ITestService, TestService>();

            services.AddShriek().AddServiceProxy(option =>
            {
                option.AddTcpProxy<ITestService>("localhost", 9091, config, true);
            });

            var provider = services.BuildAspectCoreServiceProvider();

            var client = provider.GetService<ITestService>();

            Assert.IsNotNull(client);

            var result = client.GetName("elderjames");

            Assert.AreEqual(result, "elderjames");
        }

        [ServiceContract]
        public interface ITestService
        {
            [OperationContract]
            Task<string> GetName(string name);
        }

        public class TestService : ITestService
        {
            public async Task<string> GetName(string name)
            {
                return await Task.FromResult(name);
            }
        }
    }
}
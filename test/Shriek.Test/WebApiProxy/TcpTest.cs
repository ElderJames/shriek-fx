using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TcpServiceCore.Attributes;
using TcpServiceCore.Client;
using TcpServiceCore.Communication;

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

            var client = await ChannelFactory<ITestService>.CreateProxy("localhost", 9091, config, true);

            Assert.IsNotNull(client);
        }

        [ServiceContract]
        public interface ITestService
        {
            [OperationContract]
            Task<string> GetName(string name);
        }
    }
}
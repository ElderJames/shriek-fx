using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shriek.WebApi.Proxy;

namespace Shriek.Test.WebApiProxy
{
    [TestClass]
    public class GetNotEncodingPlusTest
    {
        private const string HelloWorld = "Hello world!";
        private Mock<IInvocation> _invokation;
        private Mock<IHttpClient> _client;
        private IGreetings apiClient;

        [HttpHost("http://localhost")]
        [Route("api")]
        public interface IGreetings
        {
            [Route("{number:int}")]
            [HttpGet]
            [JsonReturn]
            Task<string> Hello(int number);
        }

        [TestInitialize]
        public void RunInvokation()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"\"{HelloWorld}\"", Encoding.UTF8, "application/json")
            };

            _client = new Mock<IHttpClient>();
            _client.Setup(s => s.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(httpResponseMessage);

            _invokation = new Mock<IInvocation>();
            _invokation.SetupGet(i => i.Method).Returns(typeof(IGreetings).GetMethod("Hello"));
            _invokation.SetupGet(i => i.Arguments).Returns(new object[] { 1555 });
            _invokation.SetupGet(i => i.Proxy).Returns(typeof(IGreetings));
            _invokation.SetupProperty(i => i.ReturnValue);

            var client = new HttpApiClient(_client.Object);
            apiClient = client.GetHttpApi<IGreetings>("http://localhost");
            ((IInterceptor)client).Intercept(_invokation.Object);
        }

        [TestMethod]
        public async Task VerifyReturnValue()
        {
            Assert.IsNotNull(_invokation.Object.ReturnValue);
            var result = await ((Task<string>)_invokation.Object.ReturnValue).ConfigureAwait(false);

            Assert.AreEqual(HelloWorld, result);
        }

        [TestMethod]
        public void VerifyRequest()
        {
            _client.Verify(c =>
                c.SendAsync(It.Is<HttpRequestMessage>(m =>
                    m.Method == HttpMethod.Get &&
                    m.RequestUri == new Uri("http://localhost/api/1555"))));
        }
    }
}
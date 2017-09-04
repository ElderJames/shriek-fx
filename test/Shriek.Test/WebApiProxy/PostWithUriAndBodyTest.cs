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
    public class PostWithUriAndBodyTest
    {
        private Mock<IInvocation> _invokation;
        private Mock<IHttpClient> _client;

        private ITestInterface apiClient;

        [HttpHost("http://localhost")]
        public interface ITestInterface
        {
            [HttpPost("/find/{type}"), JsonReturn]
            Task<TestResponse> Find(string type, [JsonContent] TestRequest request);
        }

        public class TestRequest
        {
            public string Query { get; set; }
        }

        public class TestResponse
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [TestInitialize]
        public void RunInvokation()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"Name\": \"John Doe\", \"Age\":47}", Encoding.UTF8, "application/json")
            };

            _client = new Mock<IHttpClient>();
            _client.Setup(s => s.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(httpResponseMessage);

            _invokation = new Mock<IInvocation>();
            _invokation.SetupGet(i => i.Method).Returns(typeof(ITestInterface).GetMethod(nameof(ITestInterface.Find)));
            _invokation.SetupGet(i => i.Arguments).Returns(new object[] { "theType", new TestRequest { Query = "name is SomeValue" } });
            _invokation.SetupGet(i => i.Proxy).Returns(typeof(ITestInterface));
            _invokation.SetupProperty(i => i.ReturnValue);

            var client = new HttpApiClient(_client.Object);
            apiClient = client.GetHttpApi<ITestInterface>("http://localhost");
            ((IInterceptor)client).Intercept(_invokation.Object);
        }

        [TestMethod]
        public async Task VerifyReturnValue()
        {
            var result = await apiClient.Find("theType", new TestRequest()
            {
                Query = "name is SomeValue"
            });

            Assert.IsNotNull(result);
            Assert.AreEqual("John Doe", result.Name);
            Assert.AreEqual(47, result.Age);
        }

        [TestMethod]
        public void VerifyRequest()
        {
            _client.Verify(c =>
                c.SendAsync(It.Is<HttpRequestMessage>(m =>
                    m.Method == HttpMethod.Post &&
                    m.RequestUri == new Uri("http://localhost/find/theType") &&
                    VerifyContent(m.Content))));
        }

        private static bool VerifyContent(HttpContent content)
        {
            Assert.AreEqual("application/json", content.Headers.ContentType.MediaType);

            var body = content.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.AreEqual("{\"Query\":\"name is SomeValue\"}", body);

            return true;
        }
    }
}
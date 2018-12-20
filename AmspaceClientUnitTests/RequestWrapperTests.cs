using AmSpaceClient;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmspaceClientUnitTests
{
    [TestFixture]
    class RequestWrapperTests
    {
        private class TestModelClass
        {
            [JsonProperty(PropertyName = "property")]
            public int Proprety { get; set; }
        }
        private interface IHttpClientHandlerProtectedMembers
        {
            Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
        }

        private IRequestWrapper _requestWrapper;
        private Mock<HttpClientHandler> _moqHttpClientHandler;
        private string _testEndpoing = "https://some.place.in.internet";
        private JsonSerializerSettings _commonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "yyyy-MM-dd"
        };

        [SetUp]
        public void SetUp()
        {
            _moqHttpClientHandler = new Mock<HttpClientHandler> { CallBase = true };
            var httpclient = new HttpClient(_moqHttpClientHandler.Object);
            var wrappermoq = new Mock<RequestWrapper> { CallBase = true };
            wrappermoq.SetupGet(wrapper => wrapper.AmSpaceHttpClient).Returns(httpclient);
            _requestWrapper = wrappermoq.Object;
        }

        [Test]
        public void AddAuthHeaders_WhenCalled_AddsDefaultRequestHeadersAuthorization()
        {
            var param = new System.Net.Http.Headers.AuthenticationHeaderValue("whatever");

            _requestWrapper.AddAuthHeaders(param);

            Assert.AreEqual(_requestWrapper.AmSpaceHttpClient.DefaultRequestHeaders.Authorization, param);
        }

        [Test]
        public void AddAuthCookies_WhenCalled_AddSpecifiedCookieToContainer()
        {
            var domain = "test.test";
            var uri = new Uri($"https://{domain}");
            var cookie = new Cookie("test", "test", "/", domain);

            _requestWrapper.AddAuthCookies(uri, cookie);

            Assert.Contains(cookie, _requestWrapper.CookieContainer.GetCookies(uri));
        }

        [Test]
        public void GetAsyncWrapperT_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Get 
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = _requestWrapper.GetAsyncWrapper<TestModelClass>(_testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        }

        [Test]
        public void GetAsyncWrapperT_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            _requestWrapper.HttpResponcePolicy = policyMock.Object;

            var result = _requestWrapper.GetAsyncWrapper<TestModelClass>(_testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task GetAsyncWrapperT_WhenReceiveValidJson_ReturnsCorrectlyDeserealizedObject()
        {
            var incomingJson = "{\"property\": \"0\"}";
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(incomingJson) };
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var expectedObject = new TestModelClass { Proprety = 0 };
            
            var result = await _requestWrapper.GetAsyncWrapper<TestModelClass>(_testEndpoing);

            Assert.AreEqual(expectedObject.Proprety, result.Proprety);
        }

        [Test]
        public void GetAsyncWrapperT_WhenReceiveUnsuccessStatusCode_Throws()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));

            Assert.ThrowsAsync<ArgumentException>(async () => await _requestWrapper.GetAsyncWrapper<TestModelClass>(_testEndpoing));
        }

        [Test]
        public void GetAsyncWrapper_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Get
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = _requestWrapper.GetAsyncWrapper(_testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        }

        [Test]
        public void GetAsyncWrapper_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            _requestWrapper.HttpResponcePolicy = policyMock.Object;

            var result = _requestWrapper.GetAsyncWrapper(_testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task GetAsyncWrapper_WhenCalled_ReturnsUnmodifiedHttpResponce()
        {
            var incomingJson = "{\"[\"test\"]\"}";
            var expectedObject = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(incomingJson) };
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expectedObject));

            var result = await _requestWrapper.GetAsyncWrapper(_testEndpoing);

            Assert.AreEqual(expectedObject, result);
        }

        [Test]
        public void GetAsyncWrapper_WhenReceiveUnsucessStatusCode_DoesNotThrows()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));

            Assert.DoesNotThrowAsync(async () => await _requestWrapper.GetAsyncWrapper(_testEndpoing));
        }
        [Test]
        public void PutAsyncWrapperT_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testModel = new TestModelClass { Proprety = 0 };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Put
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = _requestWrapper.PutAsyncWrapper(testModel, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PutAsyncWrapperT_WhenCalled_UseCorrectMessageContent()
        {
            HttpRequestMessage sentMessage = default;
            var testModel = new TestModelClass { Proprety = 0 };
            var expectedContent = new StringContent(JsonConvert.SerializeObject(testModel, Formatting.None, _commonSerializerSettings));
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => sentMessage = message);

            var result = _requestWrapper.PutAsyncWrapper(testModel, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), await sentMessage.Content.ReadAsStringAsync());
        }

        [Test]
        public void PutAsyncWrapperT_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testModel = new TestModelClass { Proprety = 0 };

            var result = _requestWrapper.PutAsyncWrapper(testModel, _testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task PutAsyncWrapperT_WhenReceiveSucessStatusCode_ReturnTrue()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.Created);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var model = new TestModelClass { Proprety = 0 };

            var result = await _requestWrapper.PutAsyncWrapper(model, _testEndpoing);

            Assert.IsTrue(result);
        }

        [Test]
        public void PutAsyncWrapperT_WhenReceiveUnsucessStatusCode_Throws()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var model = new TestModelClass { Proprety = 0 };

            Assert.ThrowsAsync<ArgumentException>(async () => await _requestWrapper.PutAsyncWrapper(model, _testEndpoing));
        }

        [Test]
        public void PutAsyncWrapperTInTOut_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testModel = new TestModelClass { Proprety = 0 };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Put
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = _requestWrapper.PutAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PutAsyncWrapperTInTOut_WhenCalled_UseCorrectMessageContent()
        {
            HttpRequestMessage sentMessage = default;
            var testModel = new TestModelClass { Proprety = 0 };
            var expectedContent = new StringContent(JsonConvert.SerializeObject(testModel, Formatting.None, _commonSerializerSettings));
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => sentMessage = message);

            var result = _requestWrapper.PutAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), await sentMessage.Content.ReadAsStringAsync());
        }

        [Test]
        public void PutAsyncWrapperTInTOut_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testModel = new TestModelClass { Proprety = 0 };

            var result = _requestWrapper.PutAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task PutAsyncWrapperTInTOut_WhenReceiveValidJson_ReturnsCorrectlyDeserealizedObject()
        {
            var modelToSend = new TestModelClass { Proprety = 0 };
            var incomingJson = "{\"property\": \"1\"}";
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(incomingJson) };
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var expectedObject = new TestModelClass { Proprety = 1 };

            var result = await _requestWrapper.PutAsyncWrapper<TestModelClass, TestModelClass>(modelToSend, _testEndpoing);

            Assert.AreEqual(expectedObject.Proprety, result.Proprety);
        }
        [Test]
        public void PutAsyncWrapperTInTOut_WhenReceiveUnsucessStatusCode_Throws()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var model = new TestModelClass { Proprety = 0 };

            Assert.ThrowsAsync<ArgumentException>(async () => await _requestWrapper.PutAsyncWrapper<TestModelClass, TestModelClass>(model, _testEndpoing));
        }

        [Test]
        public void DeleteAsyncWrapperT_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testModel = new TestModelClass { Proprety = 0 };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Delete
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = _requestWrapper.DeleteAsyncWrapper(testModel, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task DeleteAsyncWrapperT_WhenCalled_UseCorrectMessageContent()
        {
            HttpRequestMessage sentMessage = default;
            var testModel = new TestModelClass { Proprety = 0 };
            var expectedContent = new StringContent(JsonConvert.SerializeObject(testModel, Formatting.None, _commonSerializerSettings));
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => sentMessage = message);

            var result = _requestWrapper.DeleteAsyncWrapper(testModel, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), await sentMessage.Content.ReadAsStringAsync());
        }

        [Test]
        public void DeleteAsyncWrapperT_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testModel = new TestModelClass { Proprety = 0 };

            var result = _requestWrapper.DeleteAsyncWrapper(testModel, _testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task DeleteAsyncWrapperT_WhenReceiveSucessStatusCode_ReturnTrue()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.OK);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var model = new TestModelClass { Proprety = 0 };

            var result = await _requestWrapper.DeleteAsyncWrapper(model, _testEndpoing);

            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteAsyncWrapperT_WhenReceiveUnsucessStatusCode_Throws()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var model = new TestModelClass { Proprety = 0 };

            Assert.ThrowsAsync<ArgumentException>(async () => await _requestWrapper.DeleteAsyncWrapper(model, _testEndpoing));
        }

        [Test]
        public void DeleteAsyncWrapper_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Delete
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = _requestWrapper.DeleteAsyncWrapper(_testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public void DeleteAsyncWrapper_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            _requestWrapper.HttpResponcePolicy = policyMock.Object;

            var result = _requestWrapper.DeleteAsyncWrapper(_testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task DeleteAsyncWrapper_WhenReceiveSucessStatusCode_ReturnTrue()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.OK);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));

            var result = await _requestWrapper.DeleteAsyncWrapper(_testEndpoing);

            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteAsyncWrapper_WhenReceiveUnsucessStatusCode_Throws()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));

            Assert.ThrowsAsync<ArgumentException>(async () => await _requestWrapper.DeleteAsyncWrapper(_testEndpoing));
        }

        [Test]
        public void PostAsyncWrapperTInTOut_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testModel = new TestModelClass { Proprety = 0 };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Post
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = _requestWrapper.PostAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PostAsyncWrapperTInTOut_WhenCalled_UseCorrectMessageContent()
        {
            HttpRequestMessage sentMessage = default;
            var testModel = new TestModelClass { Proprety = 0 };
            var expectedContent = new StringContent(JsonConvert.SerializeObject(testModel, Formatting.None, _commonSerializerSettings));
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => sentMessage = message);

            var result = _requestWrapper.PostAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), await sentMessage.Content.ReadAsStringAsync());
        }

        [Test]
        public void PostAsyncWrapperTInTOut_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testModel = new TestModelClass { Proprety = 0 };

            var result = _requestWrapper.PostAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task PostAsyncWrapperTInTOut_WhenReceiveValidJson_ReturnsCorrectlyDeserealizedObject()
        {
            var modelToSend = new TestModelClass { Proprety = 0 };
            var incomingJson = "{\"property\": \"1\"}";
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(incomingJson) };
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var expectedObject = new TestModelClass { Proprety = 1 };

            var result = await _requestWrapper.PostAsyncWrapper<TestModelClass, TestModelClass>(modelToSend, _testEndpoing);

            Assert.AreEqual(expectedObject.Proprety, result.Proprety);
        }

        [Test]
        public void PostAsyncWrapperTInTOut_WhenReceiveUnsucessStatusCode_Throws()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var model = new TestModelClass { Proprety = 0 };

            Assert.ThrowsAsync<ArgumentException>(async () => await _requestWrapper.PostAsyncWrapper<TestModelClass, TestModelClass>(model, _testEndpoing));
        }

        [Test]
        public void PostAsyncWrapperT_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testModel = new TestModelClass { Proprety = 0 };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Post
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = _requestWrapper.PostAsyncWrapper(testModel, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PostAsyncWrapperT_WhenCalled_UseCorrectMessageContent()
        {
            HttpRequestMessage sentMessage = default;
            var testModel = new TestModelClass { Proprety = 0 };
            var expectedContent = new StringContent(JsonConvert.SerializeObject(testModel, Formatting.None, _commonSerializerSettings));
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => sentMessage = message);

            var result = _requestWrapper.PostAsyncWrapper(testModel, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), await sentMessage.Content.ReadAsStringAsync());
        }

        [Test]
        public void PostAsyncWrapperT_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testModel = new TestModelClass { Proprety = 0 };

            var result = _requestWrapper.PostAsyncWrapper(testModel, _testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task PostAsyncWrapperT_WhenReceiveSucessStatusCode_ReturnTrue()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.Created);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var model = new TestModelClass { Proprety = 0 };

            var result = await _requestWrapper.PostAsyncWrapper(model, _testEndpoing);

            Assert.IsTrue(result);
        }

        [Test]
        public void PostAsyncWrapperT_WhenReceiveUnsucessStatusCode_Throws()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var model = new TestModelClass { Proprety = 0 };

            Assert.ThrowsAsync<ArgumentException>(async () => await _requestWrapper.PostAsyncWrapper(model, _testEndpoing));
        }

    }
}

using AmSpaceClient;
using Extensions.AmSpaceClient;
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

            public override bool Equals(object obj)
            {
                return this.Proprety == (obj as TestModelClass)?.Proprety;
            }

            public override int GetHashCode()
            {
                return this.Proprety.GetHashCode();
            }
        }
        private interface IHttpClientHandlerProtectedMembers
        {
            Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
        }

        private IRequestWrapper _requestWrapper;
        private Mock<HttpMessageHandler> _moqHttpClientHandler;
        private string _testEndpoing = "https://some.place.in.internet";
        private JsonSerializerSettings _commonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "yyyy-MM-dd",
            Formatting = Formatting.None
        };
        private HttpResponseMessage _defaultResponce;

        [SetUp]
        public void SetUp()
        {
            _defaultResponce = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"property\": \"0\"}")
            };
            _moqHttpClientHandler = new Mock<HttpMessageHandler>();
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_defaultResponce));
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
        public async Task GetAsyncWrapperT_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Get 
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.GetAsyncWrapper<TestModelClass>(_testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        }

        [Test]
        public async Task GetAsyncWrapperT_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;

            var result = await _requestWrapper.GetAsyncWrapper<TestModelClass>(_testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task GetAsyncWrapperT_WhenReceiveValidJson_ReturnsCorrectlyDeserealizedObject()
        {
            var incomingJson = "{\"property\": \"1\"}";
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(incomingJson) };
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var expectedObject = new TestModelClass { Proprety = 1 };
            
            var result = await _requestWrapper.GetAsyncWrapper<TestModelClass>(_testEndpoing);

            Assert.AreEqual(expectedObject, result);
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
        public async Task GetAsyncWrapper_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Get
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.GetAsyncWrapper(_testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        }

        [Test]
        public async Task GetAsyncWrapper_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;

            var result = await _requestWrapper.GetAsyncWrapper(_testEndpoing);

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
        public async Task PutAsyncWrapperT_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testModel = new TestModelClass { Proprety = 0 };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Put
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.PutAsyncWrapper(testModel, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PutAsyncWrapperT_WhenCalled_UseCorrectMessageContent()
        {
            string sentContent = default;
            var testModel = new TestModelClass { Proprety = 0 };
            var expectedContent = new StringContent(JsonConvert.SerializeObject(testModel, _commonSerializerSettings));
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>(async (message, token) => sentContent = await message.Content.ReadAsStringAsync())
                .Returns(Task.FromResult(_defaultResponce));

            var result = await _requestWrapper.PutAsyncWrapper(testModel, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), sentContent);
        }

        [Test]
        public async Task PutAsyncWrapperT_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testModel = new TestModelClass { Proprety = 0 };

            var result = await _requestWrapper.PutAsyncWrapper(testModel, _testEndpoing);

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
        public async Task PutAsyncWrapperTInTOut_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testModel = new TestModelClass { Proprety = 0 };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Put
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.PutAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PutAsyncWrapperTInTOut_WhenCalled_UseCorrectMessageContent()
        {
            string sentContent = default;
            var testModel = new TestModelClass { Proprety = 0 };
            var expectedContent = new StringContent(JsonConvert.SerializeObject(testModel, _commonSerializerSettings));
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>(async (message, token) => sentContent = await message.Content.ReadAsStringAsync())
                .Returns(Task.FromResult(_defaultResponce));

            var result = await _requestWrapper.PutAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), sentContent);
        }

        [Test]
        public async Task PutAsyncWrapperTInTOut_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testModel = new TestModelClass { Proprety = 0 };

            var result = await _requestWrapper.PutAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

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

            Assert.AreEqual(expectedObject, result);
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
        public async Task DeleteAsyncWrapperT_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testModel = new TestModelClass { Proprety = 0 };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Delete
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.DeleteAsyncWrapper(testModel, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task DeleteAsyncWrapperT_WhenCalled_UseCorrectMessageContent()
        {
            string sentContent = default;
            var testModel = new TestModelClass { Proprety = 0 };
            var expectedContent = new StringContent(JsonConvert.SerializeObject(testModel, _commonSerializerSettings));
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>(async (message, token) => sentContent = await message.Content.ReadAsStringAsync())
                .Returns(Task.FromResult(_defaultResponce));

            var result = await _requestWrapper.DeleteAsyncWrapper(testModel, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), sentContent);
        }

        [Test]
        public async Task DeleteAsyncWrapperT_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testModel = new TestModelClass { Proprety = 0 };

            var result = await _requestWrapper.DeleteAsyncWrapper(testModel, _testEndpoing);

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
        public async Task DeleteAsyncWrapper_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Delete
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.DeleteAsyncWrapper(_testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task DeleteAsyncWrapper_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;

            var result = await _requestWrapper.DeleteAsyncWrapper(_testEndpoing);

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
        public async Task PostAsyncWrapperTInTOut_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testModel = new TestModelClass { Proprety = 0 };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Post
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.PostAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PostAsyncWrapperTInTOut_WhenCalled_UseCorrectMessageContent()
        {
            string sentContent = default;
            var testModel = new TestModelClass { Proprety = 0 };
            var expectedContent = new StringContent(JsonConvert.SerializeObject(testModel, _commonSerializerSettings));
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>(async (message, token) => sentContent = await message.Content.ReadAsStringAsync())
                .Returns(Task.FromResult(_defaultResponce));

            var result = await _requestWrapper.PostAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), sentContent);
        }

        [Test]
        public async Task PostAsyncWrapperTInTOut_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testModel = new TestModelClass { Proprety = 0 };

            var result = await _requestWrapper.PostAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

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

            Assert.AreEqual(expectedObject, result);
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
        public async Task PostAsyncWrapperT_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testModel = new TestModelClass { Proprety = 0 };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Post
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.PostAsyncWrapper(testModel, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PostAsyncWrapperT_WhenCalled_UseCorrectMessageContent()
        {
            string sentContent = default;
            var testModel = new TestModelClass { Proprety = 0 };
            var expectedContent = new StringContent(JsonConvert.SerializeObject(testModel, _commonSerializerSettings));
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>(async (message, token) => sentContent = await message.Content.ReadAsStringAsync())
                .Returns(Task.FromResult(_defaultResponce));

            var result = await _requestWrapper.PostAsyncWrapper(testModel, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), sentContent);
        }

        [Test]
        public async Task PostAsyncWrapperT_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testModel = new TestModelClass { Proprety = 0 };

            var result = await _requestWrapper.PostAsyncWrapper(testModel, _testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task PostAsyncWrapperT_WhenReceiveSucessStatusCode_ReturnTrue()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.OK);
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

        [Test]
        public async Task PostFormUrlEncodedContentAsyncWrapper_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testContent = new Dictionary<string, string> { ["test"] = "test" };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Post
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.PostFormUrlEncodedContentAsyncWrapper(testContent, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PostFormUrlEncodedContentAsyncWrapper_WhenCalled_UseCorrectMessageContent()
        {
            string sentContent = default;
            var testContent = new Dictionary<string, string> { ["test"] = "test" };
            var expectedContent = new FormUrlEncodedContent(testContent);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>(async (message, token) => sentContent = await message.Content.ReadAsStringAsync())
                .Returns(Task.FromResult(_defaultResponce));

            var result = await _requestWrapper.PostFormUrlEncodedContentAsyncWrapper(testContent, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), sentContent);
        }

        [Test]
        public async Task PostFormUrlEncodedContentAsyncWrapper_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testContent = new Dictionary<string, string> { ["test"] = "test" };

            var result = await _requestWrapper.PostFormUrlEncodedContentAsyncWrapper(testContent, _testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task PostFormUrlEncodedContentAsyncWrapper_WhenCalled_ReturnsUnmodifiedHttpResponce()
        {
            var incomingJson = "{\"[\"test\"]\"}";
            var testContent = new Dictionary<string, string> { ["test"] = "test" };
            var expectedObject = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(incomingJson) };
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expectedObject));

            var result = await _requestWrapper.PostFormUrlEncodedContentAsyncWrapper(testContent, _testEndpoing);

            Assert.AreEqual(expectedObject, result);
        }

        [Test]
        public void PostFormUrlEncodedContentAsyncWrapper_WhenReceiveUnsucessStatusCode_DoesNotThrows()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            var testContent = new Dictionary<string, string> { ["test"] = "test" };
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));

            Assert.DoesNotThrowAsync(async () => await _requestWrapper.PostFormUrlEncodedContentAsyncWrapper(testContent, _testEndpoing));
        }

        [Test]
        public async Task PostFormUrlEncodedContentAsyncWrapperTout_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testContent = new Dictionary<string, string> { ["test"] = "test" };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Post
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.PostFormUrlEncodedContentAsyncWrapper<TestModelClass>(testContent, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PostFormUrlEncodedContentAsyncWrapperTout_WhenCalled_UseCorrectMessageContent()
        {
            string sentContent = default;
            var testContent = new Dictionary<string, string> { ["test"] = "test" };
            var expectedContent = new FormUrlEncodedContent(testContent);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>(async (message, token) => sentContent = await message.Content.ReadAsStringAsync())
                .Returns(Task.FromResult(_defaultResponce));

            var result = await _requestWrapper.PostFormUrlEncodedContentAsyncWrapper<TestModelClass>(testContent, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), sentContent);
        }

        [Test]
        public async Task PostFormUrlEncodedContentAsyncWrapperTout_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testContent = new Dictionary<string, string> { ["test"] = "test" };

            var result = await _requestWrapper.PostFormUrlEncodedContentAsyncWrapper<TestModelClass>(testContent, _testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }


        [Test]
        public async Task PostFormUrlEncodedContentAsyncWrapperTout_WhenReceiveValidJson_ReturnsCorrectlyDeserealizedObject()
        {
            var testContent = new Dictionary<string, string> { ["test"] = "test" };
            var incomingJson = "{\"property\": \"1\"}";
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(incomingJson) };
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var expectedObject = new TestModelClass { Proprety = 1 };

            var result = await _requestWrapper.PostFormUrlEncodedContentAsyncWrapper<TestModelClass>(testContent, _testEndpoing);

            Assert.AreEqual(expectedObject, result);
        }
        
        [Test]
        public void PostFormUrlEncodedContentAsyncWrapperTout_WhenReceiveUnsucessStatusCode_Throws()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var testContent = new Dictionary<string, string> { ["test"] = "test" };

            Assert.ThrowsAsync<ArgumentException>(async () => await _requestWrapper.PostFormUrlEncodedContentAsyncWrapper<TestModelClass>(testContent, _testEndpoing));
        }

        [Test]
        public async Task PatchAsyncWrapperTInTOut_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testModel = new TestModelClass { Proprety = 0 };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == new HttpMethod("PATCH")
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.PatchAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PatchAsyncWrapperTInTOut_WhenCalled_UseCorrectMessageContent()
        {
            string sentContent = default;
            var testModel = new TestModelClass { Proprety = 0 };
            var expectedContent = new StringContent(JsonConvert.SerializeObject(testModel, _commonSerializerSettings));
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>(async (message, token) => sentContent = await message.Content.ReadAsStringAsync())
                .Returns(Task.FromResult(_defaultResponce));

            var result = await _requestWrapper.PatchAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), sentContent);
        }

        [Test]
        public async Task PatchAsyncWrapperTInTOut_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testModel = new TestModelClass { Proprety = 0 };

            var result = await _requestWrapper.PatchAsyncWrapper<TestModelClass, TestModelClass>(testModel, _testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task PatchAsyncWrapperTInTOut_WhenReceiveValidJson_ReturnsCorrectlyDeserealizedObject()
        {
            var modelToSend = new TestModelClass { Proprety = 0 };
            var incomingJson = "{\"property\": \"1\"}";
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(incomingJson) };
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var expectedObject = new TestModelClass { Proprety = 1 };

            var result = await _requestWrapper.PatchAsyncWrapper<TestModelClass, TestModelClass>(modelToSend, _testEndpoing);

            Assert.AreEqual(expectedObject, result);
        }
        [Test]
        public void PatchAsyncWrapperTInTOut_WhenReceiveUnsucessStatusCode_Throws()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var model = new TestModelClass { Proprety = 0 };

            Assert.ThrowsAsync<ArgumentException>(async () => await _requestWrapper.PatchAsyncWrapper<TestModelClass, TestModelClass>(model, _testEndpoing));
        }

        [Test]
        public async Task PatchAsyncWrapperT_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            var testModel = new TestModelClass { Proprety = 0 };
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == new HttpMethod("PATCH")
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.PatchAsyncWrapper(testModel, _testEndpoing);

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PatchAsyncWrapperT_WhenCalled_UseCorrectMessageContent()
        {
            string sentContent = default;
            var testModel = new TestModelClass { Proprety = 0 };
            var expectedContent = new StringContent(JsonConvert.SerializeObject(testModel, _commonSerializerSettings));
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>(async (message, token) => sentContent = await message.Content.ReadAsStringAsync())
                .Returns(Task.FromResult(_defaultResponce));

            var result = await _requestWrapper.PatchAsyncWrapper(testModel, _testEndpoing);

            Assert.AreEqual(await expectedContent.ReadAsStringAsync(), sentContent);
        }

        [Test]
        public async Task PatchAsyncWrapperT_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;
            var testModel = new TestModelClass { Proprety = 0 };

            var result = await _requestWrapper.PatchAsyncWrapper(testModel, _testEndpoing);

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task PatchAsyncWrapperT_WhenReceiveSucessStatusCode_ReturnTrue()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.Created);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var model = new TestModelClass { Proprety = 0 };

            var result = await _requestWrapper.PatchAsyncWrapper(model, _testEndpoing);

            Assert.IsTrue(result);
        }

        [Test]
        public void PatchAsyncWrapperT_WhenReceiveUnsucessStatusCode_Throws()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(incomingHttpResponce));
            var model = new TestModelClass { Proprety = 0 };

            Assert.ThrowsAsync<ArgumentException>(async () => await _requestWrapper.PatchAsyncWrapper(model, _testEndpoing));
        }

        #region Post form tests
        [Test]
        public async Task PostFormAsyncWrapperTOut_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Post
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.PostFormAsync<TestModelClass>(_testEndpoing, new List<KeyValuePair<string, string>>(), new List<FileToUpload>());

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PostFormAsyncWrapperTOut_WhenCalled_UseCorrectParametersContent()
        {
            string sentContent = default;
            var parameters = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("param1", "value1")
            };
            var files = new List<FileToUpload> {
                new FileToUpload
                {
                    Data = new byte[0],
                    FileName = "file.txt",
                    DataName = "file"
                }
            };
            
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>(async (message, token) => 
                    sentContent = await (message.Content as MultipartFormDataContent).OfType<StringContent>().First(_ => _.Headers.ContentDisposition.Name == "param1").ReadAsStringAsync())
                .ReturnsAsync(_defaultResponce);

            var result = await _requestWrapper.PostFormAsync<TestModelClass>(_testEndpoing, parameters, files);

            Assert.AreEqual(parameters[0].Value, sentContent);
        }

        [Test]
        public async Task PostFormAsyncWrapperTOut_WhenCalled_UseCorrectFileContent()
        {
            byte[] sentContent = default;
            var parameters = new List<KeyValuePair<string, string>>();
            var files = new List<FileToUpload> {
                new FileToUpload
                {
                    Data = new byte[] { 0x25, 0x20 },
                    FileName = "file.txt",
                    DataName = "file"
                }
            };

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>(async (message, token) =>
                    sentContent = await (message.Content as MultipartFormDataContent).OfType<StreamContent>().First(_ => _.Headers.ContentDisposition.Name == "file").ReadAsByteArrayAsync())
                .ReturnsAsync(_defaultResponce);

            var result = await _requestWrapper.PostFormAsync<TestModelClass>(_testEndpoing, parameters, files);

            Assert.AreEqual(files.First().Data, sentContent);
        }

        [Test]
        public async Task PostFormAsyncWrapperTOut_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;

            var result = await _requestWrapper.PostFormAsync<TestModelClass>(_testEndpoing, new List<KeyValuePair<string, string>>(), new List<FileToUpload>());

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public void PostFormAsyncWrapperTOut_WhenReceiveUnsucessStatusCode_Throws()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(incomingHttpResponce);

            Assert.ThrowsAsync<ArgumentException>(async () => await _requestWrapper.PostFormAsync<TestModelClass>(_testEndpoing, new List<KeyValuePair<string, string>>(), new List<FileToUpload>()));
        }
        #endregion
        #region Put form tests
        [Test]
        public async Task PutFormAsyncWrapperTOut_WhenCalled_MakeUseOfHttpClientWithCorrectMethodAndEndpoint()
        {
            Expression<Func<HttpRequestMessage, bool>> verifyMessageMatch = message => message.Method == HttpMethod.Put
                                                                                    && message.RequestUri.OriginalString == _testEndpoing;

            var result = await _requestWrapper.PutFormAsync<TestModelClass>(_testEndpoing, new List<KeyValuePair<string, string>>(), new List<FileToUpload>());

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Verify(handler => handler.SendAsync(It.Is(verifyMessageMatch), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task PutFormAsyncWrapperTOut_WhenCalled_UseCorrectParametersContent()
        {
            string sentContent = default;
            var parameters = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("param1", "value1")
            };
            var files = new List<FileToUpload> {
                new FileToUpload
                {
                    Data = new byte[0],
                    FileName = "file.txt",
                    DataName = "file"
                }
            };

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>(async (message, token) =>
                    sentContent = await (message.Content as MultipartFormDataContent).OfType<StringContent>().First(_ => _.Headers.ContentDisposition.Name == "param1").ReadAsStringAsync())
                .ReturnsAsync(_defaultResponce);

            var result = await _requestWrapper.PutFormAsync<TestModelClass>(_testEndpoing, parameters, files);

            Assert.AreEqual(parameters[0].Value, sentContent);
        }

        [Test]
        public async Task PutFormAsyncWrapperTOut_WhenCalled_UseCorrectFileContent()
        {
            byte[] sentContent = default;
            var parameters = new List<KeyValuePair<string, string>>();
            var files = new List<FileToUpload> {
                new FileToUpload
                {
                    Data = new byte[] { 0x25, 0x20 },
                    FileName = "file.txt",
                    DataName = "file"
                }
            };

            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback<HttpRequestMessage, CancellationToken>(async (message, token) =>
                    sentContent = await (message.Content as MultipartFormDataContent).OfType<StreamContent>().First(_ => _.Headers.ContentDisposition.Name == "file").ReadAsByteArrayAsync())
                .ReturnsAsync(_defaultResponce);

            var result = await _requestWrapper.PutFormAsync<TestModelClass>(_testEndpoing, parameters, files);

            Assert.AreEqual(files.First().Data, sentContent);
        }

        [Test]
        public async Task PutFormAsyncWrapperTOut_WhenCalled_MakeUseOfIAsyncPolicy()
        {
            var policyMock = new Mock<IAsyncPolicy<HttpResponseMessage>>();
            policyMock.Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>())).Returns(Task.FromResult(_defaultResponce));
            _requestWrapper.HttpResponcePolicy = policyMock.Object;

            var result = await _requestWrapper.PutFormAsync<TestModelClass>(_testEndpoing, new List<KeyValuePair<string, string>>(), new List<FileToUpload>());

            policyMock.Verify(policy => policy.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.AtLeastOnce);
        }

        [Test]
        public void PutFormAsyncWrapperTOut_WhenReceiveUnsucessStatusCode_Throws()
        {
            var incomingHttpResponce = new HttpResponseMessage(HttpStatusCode.NotFound);
            _moqHttpClientHandler.Protected().As<IHttpClientHandlerProtectedMembers>()
                .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(incomingHttpResponce);

            Assert.ThrowsAsync<ArgumentException>(async () => await _requestWrapper.PutFormAsync<TestModelClass>(_testEndpoing, new List<KeyValuePair<string, string>>(), new List<FileToUpload>()));
        }
        #endregion
    }
}

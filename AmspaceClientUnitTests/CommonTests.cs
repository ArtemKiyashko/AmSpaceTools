using System;
using System.Net;
using AmSpaceClient;
using Moq;
using NUnit;
using NUnit.Framework;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using AmSpaceModels;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Globalization;
using AmspaceClientUnitTests.Extensions;
using System.Windows.Media;
using System.IO;

namespace AmspaceClientUnitTests
{
    [TestFixture]
    public class CommonTests
    {
        private Mock<AmSpaceClient.AmSpaceHttpClient> _amSpaceClientMock;
        private Mock<IRequestWrapper> _requestsWrapper;
        private AmSpaceClient.AmSpaceHttpClient _amSpaceClient;

        [SetUp]
        public void SetUp()
        {
            _requestsWrapper = new Mock<IRequestWrapper>();
            _amSpaceClientMock = new Mock<AmSpaceClient.AmSpaceHttpClient>();
            _amSpaceClientMock.SetupGet(c => c.Endpoints).Returns(new ApiEndpoints("https://a.b"));
            _amSpaceClientMock.SetupGet(c => c.RequestWrapper).Returns(_requestsWrapper.Object);
            _amSpaceClient = _amSpaceClientMock.Object;
        }

        [Test]
        public async Task LoginRequestAsync_WhenCalled_UseCorrectEndpoint()
        {
            var loginResult = new LoginResult();
            var secureString = new SecureString();
            var amspaceEnv = new AmSpaceEnvironment { BaseAddress = "http://b.c" };
            _requestsWrapper
                .Setup(rw => rw.PostAsyncWrapper<LoginResult>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(Task.FromResult(loginResult));

            var result = await _amSpaceClient.LoginRequestAsync("a", secureString, amspaceEnv);
            var endpoint = _amSpaceClient.Endpoints.TokenEndpoint;

            _requestsWrapper.Verify(rw => rw.PostAsyncWrapper<LoginResult>(endpoint, It.IsAny<FormUrlEncodedContent>()));
        }

        [Test]
        public async Task LoginRequestAsync_WhenCalledWithOkCredentials_ReturnsTrue()
        {
            var loginResult = new LoginResult();
            _requestsWrapper
                .Setup(rw => rw.PostAsyncWrapper<LoginResult>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(Task.FromResult(loginResult));
            var secureString = new SecureString();
            var amspaceEnv = new AmSpaceEnvironment {BaseAddress = "http://a.b"};

            var result = _amSpaceClient.LoginRequestAsync("a", secureString, amspaceEnv);

            Assert.IsTrue(await result);
        }

        [Test]
        public void LoginRequestAsync_WhenCalledWithWrongCredentials_Throws()
        {
            _requestsWrapper
                .Setup(rw => rw.PostAsyncWrapper<LoginResult>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Throws(new Exception());
            var secureString = new SecureString();
            var amspaceEnv = new AmSpaceEnvironment { BaseAddress = "http://a.b" };

            Task call() => _amSpaceClient.LoginRequestAsync("a", secureString, amspaceEnv);

            Assert.ThrowsAsync<Exception>(call);
        }

        [Test]
        public async Task LoginRequestAsync_WhenCalledWithOkCredentials_CallsAddAuthCookiesAndAddAuthHeaders()
        {
            var loginResult = new LoginResult();
            _requestsWrapper
                .Setup(rw => rw.PostAsyncWrapper<LoginResult>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(Task.FromResult(loginResult));
            var secureString = new SecureString();
            var amspaceEnv = new AmSpaceEnvironment { BaseAddress = "http://a.b" };

            await _amSpaceClient.LoginRequestAsync("a", secureString, amspaceEnv);

            _requestsWrapper.Verify( rw => rw.AddAuthCookies(It.IsAny<Uri>(), It.IsAny<Cookie>()), Times.AtLeastOnce);
            _requestsWrapper.Verify(rw => rw.AddAuthHeaders(It.IsAny<AuthenticationHeaderValue>()), Times.AtLeastOnce);
        }

        [Test]
        public void GetAvatarAsync_WhenCalledNotFromProductionEnvironment_ReturnsDefaultAvatar()
        {
            var response1 = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response1.Content = new StringContent("a");
            var response2 = new HttpResponseMessage(HttpStatusCode.OK);
            response2.Content = new ByteArrayContent(ImageExtensions.CreateRandomBitmap().ToArray());

            _requestsWrapper
                .SetupSequence(rw => rw.GetAsyncWrapper(It.IsAny<string>()))
                .Returns(Task.FromResult(response1))
                .Returns(Task.FromResult(response2));

            var result = _amSpaceClient.GetAvatarAsync("a");

            _requestsWrapper.Verify(rw => rw.GetAsyncWrapper(It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public void GetAvatarAsync_WhenCalled_ReturnsBitmapSourceObjectInstance()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            _requestsWrapper
                .SetupSequence(rw => rw.GetAsyncWrapper(It.IsAny<string>()))
                .Returns(Task.FromResult(response));

            var result = _amSpaceClient.GetAvatarAsync("a");

            Assert.That(result, Is.TypeOf(typeof(Task<BitmapSource>)));
        }


    }
}

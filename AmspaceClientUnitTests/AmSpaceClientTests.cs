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

namespace AmspaceClientUnitTests
{
    [TestFixture]
    public class AmSpaceClientTests
    {
        private IAmSpaceClient _amSpaceClient;
        private Mock<IRequestsWrapper> _requestsWrapper;

        [SetUp]
        public void SetUp()
        {
            _requestsWrapper = new Mock<IRequestsWrapper>();
            _amSpaceClient = new AmSpaceClient.AmSpaceClient() {RequestsWrapper = _requestsWrapper.Object};
        }

        [Test]
        public void LoginRequestAsync_WhenCalledWithOkCredentials_ReturnsTrue()
        {
            var loginResult = new LoginResult();
            _requestsWrapper
                .Setup(rw => rw.PostAsyncWrapper<LoginResult>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Returns(Task.FromResult(loginResult));
            var secureString = new SecureString();
            var amspaceEnv = new AmSpaceEnvironment {BaseAddress = "http://a.b"};

            var result = _amSpaceClient.LoginRequestAsync("a", secureString, amspaceEnv);

            Assert.IsTrue(result.Result);
        }

        [Test]
        public void LoginRequestAsync_WhenCalledWithWrongCredentials_Throws()
        {
            _requestsWrapper
                .Setup(rw => rw.PostAsyncWrapper<LoginResult>(It.IsAny<string>(), It.IsAny<FormUrlEncodedContent>()))
                .Throws(new Exception());
            var secureString = new SecureString();
            var amspaceEnv = new AmSpaceEnvironment { BaseAddress = "http://a.b" };

            AsyncTestDelegate call = () => _amSpaceClient.LoginRequestAsync("a", secureString, amspaceEnv);

            Assert.ThrowsAsync<Exception>(call);
        }
    }
}

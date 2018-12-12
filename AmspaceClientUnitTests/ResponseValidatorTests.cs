using System;
using System.Net.Http;
using Moq;
using NUnit;
using NUnit.Framework;
using AmSpaceClient;
using System.Threading.Tasks;
using AmSpaceModels.Idp;

namespace AmspaceClientUnitTests
{
    [TestFixture]
    public class ResponseValidatorTests
    {
        private HttpResponseMessage _messageToValidate;

        [SetUp]
        public void SetUp()
        {
            _messageToValidate = new HttpResponseMessage();
        }

        [Test]
        public async Task ValidateAsync_WhenCalledWithOkStatusCode_ReturnsTrue()
        {
            _messageToValidate.StatusCode = System.Net.HttpStatusCode.OK;
            _messageToValidate.Content = new StringContent("test");

            var result = await _messageToValidate.ValidateAsync();

            Assert.That(result == true);
        }

        [Test]
        [TestCase("[\"ERROR\"]", "ERROR")]
        [TestCase("[\"ERROR\", \"another error\"]", "ERROR\nanother error")]
        [TestCase("{\"error\" : \"i can't take this any more\"}", "error: i can't take this any more")]
        [TestCase("{\"errors\" : [\"ERROR\", \"another error\"]}", "errors: \nERROR\nanother error")]
        [TestCase("{\"errors\" : {\"ERROR\": \"error text\"}}", "errors: \nERROR: error text")]
        public void ValidateAsync_WhenCalledWithNotOkStatusCode_ThrowsWithCorrectMessage(string incomingJson, string expectedMessage)
        {
            _messageToValidate.StatusCode = System.Net.HttpStatusCode.Unauthorized;
            _messageToValidate.Content = new StringContent(incomingJson);

            AsyncTestDelegate throws = async () => await _messageToValidate.ValidateAsync();

            var exception = Assert.ThrowsAsync<ArgumentException>(throws);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public async Task ValidateAsyncTOutput_WhenCalledWithOkStatusCode_ReturnsDeserializedObject()
        {
            _messageToValidate.StatusCode = System.Net.HttpStatusCode.OK;
            _messageToValidate.Content = new StringContent("{\"value\": 123, \"display_name\": \"test\"}");
            var expected = new ActionType { Value = 123, DisplayName = "test" };

            var result = await _messageToValidate.ValidateAsync<ActionType>();

            Assert.That(result.Value == expected.Value && result.DisplayName == expected.DisplayName);
        }

        [Test]
        [TestCase("[\"ERROR\"]", "ERROR")]
        [TestCase("[\"ERROR\", \"another error\"]", "ERROR\nanother error")]
        [TestCase("{\"error\" : \"i can't take this any more\"}", "error: i can't take this any more")]
        [TestCase("{\"errors\" : [\"ERROR\", \"another error\"]}", "errors: \nERROR\nanother error")]
        [TestCase("{\"errors\" : {\"ERROR\": \"error text\"}}", "errors: \nERROR: error text")]
        public void ValidateAsyncTOutput_WhenCalledWithNotOkStatusCode_ThrowsWithCorrectMessage(string incomingJson, string expectedMessage)
        {
            _messageToValidate.StatusCode = System.Net.HttpStatusCode.Unauthorized;
            _messageToValidate.Content = new StringContent(incomingJson);

            AsyncTestDelegate throws = async () => await _messageToValidate.ValidateAsync<ActionType>();

            var exception = Assert.ThrowsAsync<ArgumentException>(throws);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}

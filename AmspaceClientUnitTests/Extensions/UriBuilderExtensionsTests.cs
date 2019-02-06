using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmSpaceClient.Extensions;

namespace AmspaceClientUnitTests.Extensions
{
    [TestFixture]
    public class UriBuilderExtensionsTests
    {
        private UriBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new UriBuilder();
        }

        [Test]
        [TestCase("José Parés Gutiérrez", ExpectedResult = "?query=Jos%C3%A9%20Par%C3%A9s%20Guti%C3%A9rrez")]
        public string AddQuery_WhenCalled_ShouldEncodeParametersWIthoutUnicodeEntitites(string value)
        {
            _builder.AddQuery("query", value);
            return _builder.Query;
        }
    }
}

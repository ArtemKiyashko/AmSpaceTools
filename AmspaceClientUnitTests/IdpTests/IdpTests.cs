using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmSpaceClient;
using AmSpaceModels;
using AmSpaceModels.Idp;
using AmSpaceModels.Organization;
using Moq;
using NUnit.Framework;

namespace AmspaceClientUnitTests.IdpTests
{
    [TestFixture]
    public class IdpTests
    {
        private Mock<AmSpaceClient.AmSpaceHttpClient> _amSpaceClientMock;
        private Mock<IRequestWrapper> _requestsWrapper;
        private IAmSpaceClient _amSpaceClient;

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
        public async Task GetLevelsAsync_WhenCalled_ReturnsIenumerableOfLevels()
        {
            var value = new List<Level> { new Level(), new Level() } as IEnumerable<Level>;
            _requestsWrapper
                .Setup(rw => rw.GetAsyncWrapper<IEnumerable<Level>>(It.IsAny<string>())).
                Returns(Task.FromResult(value));

            var result = _amSpaceClient.GetLevelsAsync();

            Assert.IsInstanceOf<IEnumerable<Level>>(await result);
        }

        [Test]
        public async Task GetLevelsAsync_WhenCalled_ReturnsExectReceivedAmountOfLevels()
        {
            var value = new List<Level> { new Level(), new Level(), new Level()} as IEnumerable<Level>;
            _requestsWrapper
                .Setup(rw => rw.GetAsyncWrapper<IEnumerable<Level>>(It.IsAny<string>())).
                Returns(Task.FromResult(value));

            var result = _amSpaceClient.GetLevelsAsync();

            Assert.That((await result).Count() == value.Count());
        }


        [Test]
        public async Task GetCompetenciesAsync_WhenCalled_ReturnsIEnumerableOfCompetency()
        {
            var levels = new List<Level> { new Level() } as IEnumerable<Level>;
            var pager = new CompetencyPager { Results = new List<Competency>() };

            _requestsWrapper
                .Setup(rw => rw.GetAsyncWrapper<CompetencyPager>(It.IsAny<string>()))
                .Returns(Task.FromResult(pager));

            var result = _amSpaceClient.GetCompetenciesAsync();

            Assert.IsInstanceOf<IEnumerable<Competency>>(await result);
        }

        [Test]
        public async Task GetCompetenciesAsync_WhenCalled_ReturnContentFromEachPageOfPagers()
        {
            var pager1 = new CompetencyPager {
                Results = new List<Competency> { new Competency { Id = 1 } },
                Next = "a"
            };
            var pager2 = new CompetencyPager
            {
                Results = new List<Competency>
                {
                    new Competency {Id = 2},
                    new Competency {Id = 3}
                }
            };

            _requestsWrapper
                .SetupSequence(rw => rw.GetAsyncWrapper<CompetencyPager>(It.IsAny<string>()))
                .Returns(Task.FromResult(pager1))
                .Returns(Task.FromResult(pager2));

            var result = await _amSpaceClient.GetCompetenciesAsync();
            var allCompetency = pager1.Results;
            allCompetency.AddRange(pager2.Results);

            Assert.That(result.SequenceEqual(allCompetency));
        }


    }
}

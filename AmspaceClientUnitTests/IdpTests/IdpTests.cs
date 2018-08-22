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
        public async Task GetLevelsAsync_WhenCalled_ReturnsIenumerableOfLevelsInstance()
        {
            var value = new List<Level> { new Level(), new Level() } as IEnumerable<Level>;
            _requestsWrapper
                .Setup(rw => rw.GetAsyncWrapper<IEnumerable<Level>>(It.IsAny<string>())).
                Returns(Task.FromResult(value));

            var result = _amSpaceClient.GetLevelsAsync();

            Assert.IsInstanceOf<IEnumerable<Level>>(await result);
        }

        [Test]
        public void GetLevelsAsync_WhenCalled_UseCorrectEndpoint()
        {
            var endpoint = _amSpaceClient.Endpoints.LevelsEndpoint;

            var result = _amSpaceClient.GetLevelsAsync();

            _requestsWrapper.Verify(wr => wr.GetAsyncWrapper<IEnumerable<Level>>(endpoint));
        }

        [Test]
        public async Task GetLevelsAsync_WhenCalled_ReturnsAllReceivedLevels()
        {
            var value = new List<Level> { new Level(), new Level(), new Level()} as IEnumerable<Level>;
            _requestsWrapper
                .Setup(rw => rw.GetAsyncWrapper<IEnumerable<Level>>(It.IsAny<string>())).
                Returns(Task.FromResult(value));

            var result = await _amSpaceClient.GetLevelsAsync();

            Assert.That(result.Count() == value.Count());
            Assert.That(result.SequenceEqual(value));
        }

        [Test]
        public async Task GetCompetenciesAsync_WhenCalled_ReturnsIEnumerableOfCompetencyInstance()
        {
            var pager = new CompetencyPager { Results = new List<Competency>() };
            _requestsWrapper
                .Setup(rw => rw.GetAsyncWrapper<CompetencyPager>(It.IsAny<string>()))
                .Returns(Task.FromResult(pager));

            var result = _amSpaceClient.GetCompetenciesAsync();

            Assert.IsInstanceOf<IEnumerable<Competency>>(await result);
        }

        [Test]
        public void GetCompetenciesAsync_WhenCalled_UseCorrectEndpoint()
        {
            var endpoint = _amSpaceClient.Endpoints.CompetencyAdminEndpoint;

            var result = _amSpaceClient.GetCompetenciesAsync();

            _requestsWrapper.Verify(wr => wr.GetAsyncWrapper<CompetencyPager>(endpoint));
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

        [Test]
        public async Task GetCompetenciesAsync_WhenCalled_ReturnCompetenciesWithFilledLevelProperty()
        {
            var pager = new CompetencyPager { Results = new List<Competency> { new Competency { LevelId = 1 } } };
            var levels = new List<Level> { new Level { Id = 1 } } as IEnumerable<Level>;
            _requestsWrapper
                .Setup(rw => rw.GetAsyncWrapper<CompetencyPager>(It.IsAny<string>()))
                .Returns(Task.FromResult(pager));
            _requestsWrapper
                .Setup(rw => rw.GetAsyncWrapper<IEnumerable<Level>>(It.IsAny<string>())).
                Returns(Task.FromResult(levels));

            var result = await _amSpaceClient.GetCompetenciesAsync();

            Assert.That(result.ElementAtOrDefault(0)?.Level, Is.Not.Null);
        }

        [Test]
        public async Task GetCompetencyActionsAsync_WhenCalled_ReturnsCompetencyActionInstance()
        {
            var value = new CompetencyAction();
            _requestsWrapper
                .Setup(rw => rw.GetAsyncWrapper<CompetencyAction>(It.IsAny<string>())).
                Returns(Task.FromResult(value));

            var result = await _amSpaceClient.GetCompetencyActionsAsync(1);

            Assert.IsInstanceOf<CompetencyAction>(result);
            Assert.That(value == result);
        }

        [Test]
        public async Task GetCompetencyActionsAsync_WhenCalled_ReturnsReceivedCompetencyAction()
        {
            var value = new CompetencyAction();
            _requestsWrapper
                .Setup(rw => rw.GetAsyncWrapper<CompetencyAction>(It.IsAny<string>())).
                Returns(Task.FromResult(value));

            var result = await _amSpaceClient.GetCompetencyActionsAsync(1);

            Assert.That(value == result);
        }

        [Test]
        public void GetCompetencyActionsAsync_WhenCalled_UseCorrectEndpoint()
        {
            long id = 1;
            var endpoint = string.Format(_amSpaceClient.Endpoints.CompetecyActionAdminEndpoint, id);

            var result = _amSpaceClient.GetCompetencyActionsAsync(id);

            _requestsWrapper.Verify(wr => wr.GetAsyncWrapper<CompetencyAction>(endpoint));
        }

    }
}

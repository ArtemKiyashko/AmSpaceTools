using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using AmSpaceModels.Organization;
using AmSpaceTools.Infrastructure;
using AmSpaceTools.Infrastructure.Providers;
using AmSpaceTools.ModelConverters;
using AutoMapper;
using ExcelWorker.Models;
using Moq;
using NUnit.Framework;

namespace AmSpaceToolsTests.Infrastructure
{
    [TestFixture]
    public class MapperTests
    {
        private IMapper _mapper;
        private Mock<IActiveDirectoryProvider> _activeDirectoryProviderMock;
        private Mock<PrincipalContext> _defaultPrincipalContextMock;
        private MapperConfiguration _mapperConfiguration;

        [SetUp]
        public void SetUp()
        {
            _defaultPrincipalContextMock = new Mock<PrincipalContext>(It.IsAny<ContextType>());
            _activeDirectoryProviderMock = new Mock<IActiveDirectoryProvider>();
            _activeDirectoryProviderMock
                .Setup(_ => _.FindOneByEmail(It.IsAny<string>()))
                .Returns(new UserPrincipal(_defaultPrincipalContextMock.Object));
            _activeDirectoryProviderMock
                .Setup(_ => _.FindAllByEmail(It.IsAny<string>()))
                .Returns(new List<Principal> { new UserPrincipal(_defaultPrincipalContextMock.Object), new UserPrincipal(_defaultPrincipalContextMock.Object) });
            _mapperConfiguration = new MapperConfiguration(cfg => {
                cfg.CreateMap<SapPersonExcelRow, ExternalAccount>().ConvertUsing(new SapPersonExcelToAmspaceConverter(_activeDirectoryProviderMock.Object));
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [Test]
        public void SapPersonExcelRow_Shoul_Be_Converted_To_ExternalAccount()
        {
            var input = new SapPersonExcelRow();
            var result = _mapper.Map<ExternalAccount>(input);
            Assert.IsInstanceOf<ExternalAccount>(result);
        }

        [Test]
        public void ExternalAccount_All_Properties_Filled()
        {
            var input = new SapPersonExcelRowComparable();
            input.Name = "test name";
            input.Surname = "test surname";
            input.Status = ContractStatus.ACTIVE;
            input.Sex = Sex.MALE;
            input.Phone = "+123456789";
            input.Nationality = "RU";
            input.Country = "PL";
            input.Mpk = 123456;
            input.Level = 3;
            input.IdentityNumber = "asda1234wqewe";
            input.ContractStartDate = DateTime.Now;
            input.ContractEndDate = DateTime.Now.AddMonths(12);
            input.ContractNumber = 1;
            input.BirthDate = new DateTime(1986, 12, 18);
            input.Email = "a@a.com";
            var result = _mapper.Map<ExternalAccount>(input);

            Assert.IsTrue(input.CompareTo(result) == 0);
        }

        [Test]
        public void Map_WhenUserPrincipalFound_ShouldSetBackendTypePropertyToActiveDirectory()
        {
            var input = new SapPersonExcelRowComparable();
            input.Email = "a@a.com";
            var result = _mapper.Map<ExternalAccount>(input);
            Assert.AreEqual(AccountBackendType.ActiveDirectory, result.BackendType);
        }

        [Test]
        public void Map_WhenUserPrincipalFound_ShouldSetBackendTypePropertyToAmSpace()
        {
            _activeDirectoryProviderMock
                .Setup(_ => _.FindOneByEmail(It.IsAny<string>()))
                .Returns(() => null);
            var input = new SapPersonExcelRowComparable();
            var result = _mapper.Map<ExternalAccount>(input);
            Assert.AreEqual(AccountBackendType.AmSpace, result.BackendType);
        }
    }
}

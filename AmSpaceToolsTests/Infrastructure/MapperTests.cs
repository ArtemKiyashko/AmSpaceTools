using System;
using AmSpaceModels.Organization;
using AmSpaceTools.Infrastructure;
using AutoMapper;
using ExcelWorker.Models;
using NUnit.Framework;

namespace AmSpaceToolsTests.Infrastructure
{
    [TestFixture]
    public class MapperTests
    {
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _mapper = Services.Container.GetInstance<IMapper>();
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
    }
}

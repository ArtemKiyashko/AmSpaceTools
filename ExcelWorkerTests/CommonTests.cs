using NUnit.Framework;
using System;
using ExcelWorker;
using ExcelWorker.Models;
using OfficeOpenXml;
using System.Reflection;
using System.Linq;

namespace ExcelWorkerTests
{
    [TestFixture]
    public class CommonTests
    {
        private string _filepath;
        private IExcelWorker _worker;

        [SetUp]
        public void SetUp()
        {
            _worker = new AmSpaceExcelWorker();
            _filepath = AppDomain.CurrentDomain.BaseDirectory + "..//..//Resources/example.xlsx";
            _worker.OpenFile(_filepath);
        }
        // to be updated after separating worker from repository
        [Test]
        public void ExtractData_WhenCalledWithGoalsType_ReturnListFromAllXlsRows()
        {
            var data = _worker.ExctractData<GoalExcelRow>("Goals");

            Assert.IsNotNull(data);
            Assert.That(data.Count() == 4);
        }

        [Test]
        public void ExtractData_WhenCalledWithIdpType_ReturnListFromAllXlsRows()
        {
            var data = _worker.ExctractData<KpiExcelRow>("KPIs");

            Assert.IsNotNull(data);
            Assert.That(data.Count() == 4);
        }
    }
}

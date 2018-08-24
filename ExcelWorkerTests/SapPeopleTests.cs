using ExcelWorker;
using ExcelWorker.Models;
using NUnit.Framework;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorkerTests
{
    [TestFixture]
    public class SapPeopleTests
    {
        private string _filepath;
        private IExcelWorker _worker;

        [SetUp]
        public void SetUp()
        {
            _worker = new AmSpaceExcelWorker();
            _filepath = AppDomain.CurrentDomain.BaseDirectory + "..//..//Resources/sap_people_example.xlsx";
            _worker.OpenFile(_filepath);
        }
        // to be updated after separating worker from repository
        [Test]
        public void ExtractData_WhenCalledWithSapPerson_ReturnListFromAllXlsRows()
        {
            var data = _worker.ExctractData<SapPersonExcelRow>("EXAMPLE");

            CollectionAssert.AllItemsAreNotNull(data);
        }

        [Test]
        public void GetWorkSheet_WhenCalled_ReturnExcelTable()
        {
            var data = _worker.GetWorkSheet("EXAMPLE");

            Assert.IsInstanceOf<DataTable>(data);
        }
    }
}

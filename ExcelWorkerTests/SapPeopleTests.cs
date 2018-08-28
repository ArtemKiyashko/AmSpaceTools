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
            _filepath = AppDomain.CurrentDomain.BaseDirectory + "..//..//Resources/ImportPeople_template.xlsx";
            _worker.OpenFile(_filepath);
        }
        // to be updated after separating worker from repository
        [Test]
        public void ExtractData_WhenCalledWithSapPerson_ReturnListFromAllXlsRows()
        {
            var data = _worker.ExctractData<SapPersonExcelRow>("Sheet1");

            CollectionAssert.AllItemsAreNotNull(data);
        }

        [Test]
        public void GetWorkSheet_WhenCalled_ReturnDataTable()
        {
            var data = _worker.GetWorkSheet("Sheet1");

            Assert.IsInstanceOf<DataTable>(data);
        }
    }
}

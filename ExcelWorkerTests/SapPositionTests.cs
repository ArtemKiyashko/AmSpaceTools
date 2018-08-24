using ExcelWorker;
using ExcelWorker.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorkerTests
{
    [TestFixture]
    public class SapPositionTests
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
        public void ExtractData_WhenCalledWithSapPosition_ReturnListFromAllXlsRows()
        {
            var data = _worker.ExctractData<SapPositionExcelRow>("dict. POSITION");

            CollectionAssert.AllItemsAreNotNull(data);
        }
    }
}

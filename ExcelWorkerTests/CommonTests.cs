using NUnit.Framework;
using System;
using ExcelWorker;
using ExcelWorker.Models;
using OfficeOpenXml;
using System.Reflection;
using System.Linq;
using Moq;
using ExcelWorker.Services;
using System.IO;
using AmSpaceModels.Enums;
using System.Collections.Generic;
using Moq.Protected;
using EPPlus.Core.Extensions;


namespace ExcelWorkerTests
{
    public class TestWithAttributes
    {
        [ExcelTableColumn]
        public string Property1 { get; set; }
    }

    public class TestWithoutAttributes
    {
        public string Property1 { get; set; }
    }

    [TestFixture]
    public class CommonTests
    {
        private Mock<AmSpaceExcelWorker> _amSpaceExcelWorker;
        private Mock<ISaveLocator> _amSpaceSaveLocator;
        private Mock<IFileWrapper> _amSpaceFileWrapper;
        private MemoryStream _memoryStream;

        [SetUp]
        public void SetUp()
        {
            _amSpaceFileWrapper = new Mock<IFileWrapper>();
            _memoryStream = new MemoryStream();
            _amSpaceFileWrapper.Setup(_ => _.GetStream(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>())).Returns(_memoryStream);

            _amSpaceSaveLocator = new Mock<ISaveLocator>();
            _amSpaceSaveLocator.Setup(_ => _.GetSaveLocation(It.IsAny<string>(), It.IsAny<AppDataFolders>())).Returns("filelocation");

            _amSpaceExcelWorker = new Mock<AmSpaceExcelWorker>();
            _amSpaceExcelWorker.SetupGet(_ => _.FileWrapper).Returns(_amSpaceFileWrapper.Object);
            _amSpaceExcelWorker.SetupGet(_ => _.SaveLocator).Returns(_amSpaceSaveLocator.Object);
        }


        [Test]
        public void SaveData_ShouldSaveCollections_WithoutExcelAttributes()
        {
            var collection = new List<TestWithoutAttributes> {
                new TestWithoutAttributes
                {
                    Property1 = "test"
                }
            };
            Assert.DoesNotThrow(() => _amSpaceExcelWorker.Object.SaveData("test", AppDataFolders.Reports, collection, "sheet"));
        }

        [Test]
        public void SaveData_ShouldSaveCollections_WithExcelAttributes()
        {
            var collection = new List<TestWithAttributes> {
                new TestWithAttributes
                {
                    Property1 = "test"
                }
            };
            Assert.DoesNotThrow(() => _amSpaceExcelWorker.Object.SaveData("test", AppDataFolders.Reports, collection, "sheet"));
        }

        [Test]
        public void ExtractData_ShouldReturn_CollectionWithAttributes()
        {
            var expected = new List<TestWithAttributes> {
                new TestWithAttributes
                {
                    Property1 = "test"
                }
            };

            _amSpaceExcelWorker.Object.SaveData("test", AppDataFolders.Reports, expected, "sheet");
            _amSpaceFileWrapper.Setup(_ => _.GetStream(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>())).Returns(new MemoryStream(_memoryStream.ToArray()));
            _amSpaceExcelWorker.Object.OpenFile("test");
            var actual = _amSpaceExcelWorker.Object.ExctractData<TestWithAttributes>("sheet");

            CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(TestWithAttributes));
        }

        [Test]
        public void ExtractData_ShouldReturn_SameCountAsExpected()
        {
            var expected = new List<TestWithAttributes> {
                new TestWithAttributes
                {
                    Property1 = "test"
                },
                new TestWithAttributes
                {
                    Property1 = "test2"
                }
            };

            _amSpaceExcelWorker.Object.SaveData("test", AppDataFolders.Reports, expected, "sheet");
            _amSpaceFileWrapper.Setup(_ => _.GetStream(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>())).Returns(new MemoryStream(_memoryStream.ToArray()));
            _amSpaceExcelWorker.Object.OpenFile("test");
            var actual = _amSpaceExcelWorker.Object.ExctractData<TestWithAttributes>("sheet");

            Assert.That(actual, Has.Count.EqualTo(expected.Count));
        }

        [Test]
        public void ExtractData_ShouldThrow_CollectionWithoutAttributes()
        {
            var expected = new List<TestWithoutAttributes> {
                new TestWithoutAttributes
                {
                    Property1 = "test"
                }
            };

            _amSpaceExcelWorker.Object.SaveData("test", AppDataFolders.Reports, expected, "sheet");
            _amSpaceFileWrapper.Setup(_ => _.GetStream(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>())).Returns(new MemoryStream(_memoryStream.ToArray()));
            _amSpaceExcelWorker.Object.OpenFile("test");

            Assert.Throws<ArgumentException>(() => _amSpaceExcelWorker.Object.ExctractData<TestWithoutAttributes>("sheet"));
        }

        [Test]
        public void GetWroksheet_ShouldReturn_WithoutAttributes()
        {
            var expected = new List<TestWithoutAttributes> {
                new TestWithoutAttributes
                {
                    Property1 = "test"
                }
            };

            _amSpaceExcelWorker.Object.SaveData("test", AppDataFolders.Reports, expected, "sheet");
            _amSpaceFileWrapper.Setup(_ => _.GetStream(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>())).Returns(new MemoryStream(_memoryStream.ToArray()));
            _amSpaceExcelWorker.Object.OpenFile("test");
            var actual = _amSpaceExcelWorker.Object.GetWorkSheet("sheet");
            Assert.That(actual.Rows, Has.Count.EqualTo(expected.Count));
        }
    }
}

using ExcelWorker.Models;
using AmSpaceModels.Performance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmSpaceModels.Idp;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Data;

namespace ExcelWorker
{
    public interface IExcelWorker : IDisposable
    {
        IEnumerable<IdpColumn> GetColumnDataPreview(int rowLimit);
        IEnumerable<IdpExcelRow> GetAllRows(IEnumerable<IdpColumn> columnDefinitions, bool ignoreFirstRow = true);
        void SaveData<T>(string fileName, IEnumerable<T> data, string sheetName) where T : class;
        IEnumerable<T> ExctractData<T>(string sheetName) where T : class, new();
        IEnumerable<string> GetWroksheets();
        DataTable GetWorkSheet(string sheetName);
        void OpenFile(string fileName);
    }
}

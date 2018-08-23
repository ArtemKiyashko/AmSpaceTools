using ExcelWorker.Models;
using AmSpaceModels.Performance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmSpaceModels.Idp;

namespace ExcelWorker
{
    public interface IExcelWorker
    {
        IEnumerable<IdpColumn> GetColumnDataPreview(string fileName, int rowLimit);
        IEnumerable<IdpExcelRow> GetAllRows(string fileName, IEnumerable<IdpColumn> columnDefinitions, bool ignoreFirstRow = true);
        void SaveData<T>(string fileName, IEnumerable<T> data, string sheetName) where T : class;
        IEnumerable<T> ExctractData<T>(string fileName, string sheetName) where T : class, new();
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmSpaceModels.Idp;
using AmSpaceModels.Performance;
using EPPlus.DataExtractor;
using ExcelWorker.Models;
using OfficeOpenXml;
using EPPlus.Core.Extensions;
using OfficeOpenXml.Table;
using System.Data;

namespace ExcelWorker
{
    public class AmSpaceExcelWorker : IExcelWorker
    {
        public string FileName { get; private set; }
        private FileStream _fStream { get; set; }
        private ExcelPackage _ePackage { get; set; }
        public IEnumerable<IdpColumn> GetColumnDataPreview(int rowLimit)
        {
            var result = new List<IdpColumn>();
            var ws = _ePackage.Workbook.Worksheets.First(_ => _.Hidden == eWorkSheetHidden.Visible);
            var lastColumn = ws.Dimension.End.Column;

            for (var i = 1; i < lastColumn + 1; i++)
            {
                var c = new IdpColumn
                {
                    WorkSheet = ws.Index,
                    ColumnAddress = ws.Cells[1, i].Address,
                    ColumnType = ColumnActionType.NotSpecified,
                    ColumnIndex = i,
                    ColumnData = ws.Cells[1, i, rowLimit, i].Select(_ => _.Text).ToList()
                };
                result.Add(c);
            }
            return result;
        }

        public IEnumerable<IdpExcelRow> GetAllRows(IEnumerable<IdpColumn> columnDefinitions, bool ignoreFirstRow = true)
        {
            var result = new List<IdpExcelRow>();
            foreach (var ws in _ePackage.Workbook.Worksheets)
            {
                if (ws.Hidden == eWorkSheetHidden.Hidden) continue;
                var lastRow = ws.Dimension.End.Row;
                var lastColumn = ws.Dimension.End.Column;
                for (var i = ignoreFirstRow ? 2 : 1; i < lastRow + 1; i++)
                {
                    //if first cell empty, continue from next row
                    if (ws.Cells[i, 1].Value == null) continue;

                    //if whole row empty - stop iteration on ws
                    if (ws.Cells[i, 1, i, lastColumn].All(_ => _.Value == null)) break;

                    var c = new IdpExcelRow
                    {
                        CompetencyName = ws.GetValue<string>(i, columnDefinitions.First(_ => _.ColumnType == ColumnActionType.CompetencyName).ColumnIndex),
                        CompetencyLevel = ws.GetValue<int>(i, columnDefinitions.First(_ => _.ColumnType == ColumnActionType.CompetencyLevel).ColumnIndex),
                        ActionPercentage = ws.GetValue<int>(i, columnDefinitions.First(_ => _.ColumnType == ColumnActionType.ActionPercentage).ColumnIndex),
                        ActionSourceDescription = ws.GetValue<string>(i, columnDefinitions.First(_ => _.ColumnType == ColumnActionType.SourceText).ColumnIndex),
                        Translations = new List<Translation>()
                    };
                    foreach (var translation in columnDefinitions.Where(_ => _.ColumnType == ColumnActionType.Translation))
                    {
                        c.Translations.Add(new Translation {
                            Language = translation.Language.ToString().ToLower(),
                            Name = ws.GetValue<string>(i, translation.ColumnIndex)
                        });
                    }
                    result.Add(c);
                }
            }
            return result;
        }

        public IEnumerable<T> ExctractData<T>(string sheetName) where T : class, new()
        {
            var ws = string.IsNullOrEmpty(sheetName) ?
                _ePackage.Workbook.Worksheets[1] :
                _ePackage.Workbook.Worksheets[sheetName];
            return ws.ToList<T>(options => options.SkipCastingErrors());
        }

        public IEnumerable<string> GetWorksheets()
        {
            return _ePackage.Workbook.Worksheets.Select(_ => _.Name);
        }

        public DataTable GetWorkSheet(string sheetName)
        {
            return _ePackage.Workbook.Worksheets[sheetName].ToDataTable();
        }

        public DataTable GetWorkSheet(int index)
        {
            return _ePackage.Workbook.Worksheets[index].ToDataTable();
        }

        public void OpenFile(string fileName)
        {
            Dispose();
            FileName = fileName;
            _fStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _ePackage = new ExcelPackage(_fStream);
        }

        public void Dispose()
        {
            if (_fStream != null) _fStream.Close();
            if (_ePackage != null) _ePackage.Dispose();
            _fStream = null;
            _ePackage = null;
        }

        public void SaveData<T>(string fileName, IEnumerable<T> data, string sheetName) where T : class
        {
            using (var file = new FileStream(FileName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            using (var excel = new ExcelPackage(file))
            {
                var ws = string.IsNullOrEmpty(sheetName) ?
                excel.Workbook.Worksheets[1] :
                excel.Workbook.Worksheets.Add(sheetName);
                ws.Cells["A1"].LoadFromCollectionFiltered(data, true);
                var header = ws.Cells[ws.Dimension.Start.Row, ws.Dimension.Start.Column, ws.Dimension.Start.Row, ws.Dimension.End.Column];
                header.AutoFilter = true;
                header.AutoFitColumns();
                ws.View.FreezePanes(2, 1);
                excel.Save();
            }
        }
    }
}

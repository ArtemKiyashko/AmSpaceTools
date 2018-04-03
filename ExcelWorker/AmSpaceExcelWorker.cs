using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmSpaceModels;
using OfficeOpenXml;

namespace ExcelWorker
{
    public class AmSpaceExcelWorker<T> : IExcelWorker<T>
    {
        public IEnumerable<IdpExcelColumn> GetColumnDataPreview(string fileName, int rowLimit)
        {
            using (var file = new FileStream(fileName, FileMode.Open))
            using (var excel = new ExcelPackage(file))
            {
                var result = new List<IdpExcelColumn>();
                var ws = excel.Workbook.Worksheets.First(_ => _.Hidden == eWorkSheetHidden.Visible);
                var lastColumn = ws.Dimension.End.Column;

                for (var i = 1; i < lastColumn + 1; i++)
                {
                    var c = new IdpExcelColumn();
                    c.WorkSheet = ws.Index;
                    c.ColumnAddress = ws.Cells[1, i].Address;
                    c.ColumnType = ColumnActionType.NotSpecified;
                    c.ColumnIndex = i;
                    c.ColumnData = ws.Cells[1, i, rowLimit, i].Select(_ => _.Text).ToList();
                    result.Add(c);
                }
                return result;
            }
        }

        public IEnumerable<IdpExcelRow> GetAllRows(string fileName, IEnumerable<IdpExcelColumn> columnDefinitions, bool ignoreFirstRow = true)
        {
            using (var file = new FileStream(fileName, FileMode.Open))
            using (var excel = new ExcelPackage(file))
            {
                var result = new List<IdpExcelRow>();
                foreach (var ws in excel.Workbook.Worksheets)
                {
                    if (ws.Hidden == eWorkSheetHidden.Hidden) continue;
                    var lastRow = ws.Dimension.End.Row;
                    var lastColumn = ws.Dimension.End.Column;
                    for (var i = ignoreFirstRow ? 2 : 1; i < lastRow + 1; i++)
                    {
                        var c = new IdpExcelRow();
                        c.CompetencyName = ws.GetValue<string>(i, columnDefinitions.First(_ => _.ColumnType == ColumnActionType.CompetencyName).ColumnIndex);
                        c.CompetencyLevel = ws.GetValue<int>(i, columnDefinitions.First(_ => _.ColumnType == ColumnActionType.CompetencyLevel).ColumnIndex);
                        c.ActionPercentage = ws.GetValue<int>(i, columnDefinitions.First(_ => _.ColumnType == ColumnActionType.ActionPercentage).ColumnIndex);
                        c.ActionSourceDescription = ws.GetValue<string>(i, columnDefinitions.First(_ => _.ColumnType == ColumnActionType.SourceText).ColumnIndex);
                        c.Translations = new List<Translation>();
                        foreach(var translation in columnDefinitions.Where(_ => _.ColumnType == ColumnActionType.Translation))
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
        }

        public void SaveData(string fileName, IEnumerable<T> data, string sheetName)
        {
            using (var file = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
            using (var excel = new ExcelPackage(file))
            {
                var ws = string.IsNullOrEmpty(sheetName) ? 
                    excel.Workbook.Worksheets[1] : 
                    excel.Workbook.Worksheets.Add(sheetName);
                ws.Cells["A1"].LoadFromCollection(data, true);
                var header = ws.Cells[ws.Dimension.Start.Row, ws.Dimension.Start.Column, ws.Dimension.Start.Row, ws.Dimension.End.Column];
                header.AutoFilter = true;
                header.AutoFitColumns();
                ws.View.FreezePanes(2, 1);
                excel.Save();
            }
        }

        public IEnumerable<ColumnDefinitionError> ValidateColumnDefinitions(IEnumerable<IdpExcelColumn> columnDefinitions)
        {
            var result = new List<ColumnDefinitionError>();
            if (!columnDefinitions.Any(_ => _.ColumnType == ColumnActionType.CompetencyName))
                result.Add(new ColumnDefinitionError(ColumnActionType.CompetencyName));
            if (!columnDefinitions.Any(_ => _.ColumnType == ColumnActionType.ActionPercentage))
                result.Add(new ColumnDefinitionError(ColumnActionType.ActionPercentage));
            if (!columnDefinitions.Any(_ => _.ColumnType == ColumnActionType.CompetencyLevel))
                result.Add(new ColumnDefinitionError(ColumnActionType.CompetencyLevel));
            if (!columnDefinitions.Any(_ => _.ColumnType == ColumnActionType.SourceText))
                result.Add(new ColumnDefinitionError(ColumnActionType.SourceText));
            if (!columnDefinitions.Any(_ => _.ColumnType == ColumnActionType.Translation))
                result.Add(new ColumnDefinitionError(ColumnActionType.Translation));
            return result;
        }
    }
}

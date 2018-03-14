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
    public class AmSpaceExcelWorker : IExcelWorker
    {
        public IEnumerable<IdpExcelColumn> GetData(string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Open))
            using (var excel = new ExcelPackage(file))
            {
                var result = new List<IdpExcelColumn>();
                foreach (var ws in excel.Workbook.Worksheets)
                {
                    if (ws.Hidden == eWorkSheetHidden.Hidden) continue;
                    var lastRow = ws.Dimension.End.Row;
                    var lastColumn = ws.Dimension.End.Column;
                    for (var i = 1; i < lastColumn + 1; i++)
                    {
                        var c = new IdpExcelColumn();
                        c.WorkSheet = ws.Index;
                        c.ColumntAddress = ws.Cells[1, i].Address;
                        c.ColumnType = ColumnActionType.NotSpecified;
                        c.ColumnData = ws.Cells[1, i, lastRow, i].Select(_ => _.Text).ToList();
                        result.Add(c);
                    }
                }
                return result;
            }
        }
    }
}

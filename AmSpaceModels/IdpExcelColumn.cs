using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels
{
    public class IdpExcelColumn
    {
        public string ColumntAddress { get; set; }
        public ColumnActionType ColumnType { get; set; }
        public IEnumerable<string> ColumnData { get; set; }
        public int WorkSheet { get; set; }
    }

    public enum ColumnActionType
    {
        NotSpecified = 0,
        Translation = 1,
        SourceText = 2
    }
}

using EPPlus.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker.Models
{
    public class SapPositionExcelRow
    {
        [ExcelTableColumn("SAP field SHORT")]
        public string PositionCode { get; set; }

        [ExcelTableColumn("STEXT")]
        public string PositionName { get; set; }
    }
}

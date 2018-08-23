using EPPlus.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker.Models
{
    public class KpiExcelRow
    {
        [ExcelTableColumn("Year")]
        public int Year { get; set; }
        [ExcelTableColumn("Country")]
        public string Country { get; set; }
        [ExcelTableColumn("Brand")]
        public string Brand { get; set; }
        [ExcelTableColumn("Position")]
        public string Position { get; set; }
        [ExcelTableColumn("KPI description")]
        public string KpiDescription { get; set; }
        [ExcelTableColumn("Target")]
        public string KpiTarget { get; set; }
        [ExcelTableColumn("Type")]
        public string KpiType { get; set; }
    }
}

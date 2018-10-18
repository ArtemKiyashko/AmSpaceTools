using EPPlus.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker.Models
{
    public class JobMapExcelRow
    {
        [ExcelTableColumn("Country")]
        public string Country { get; set; }
        [ExcelTableColumn("Brand")]
        public string Brand { get; set; }
        [ExcelTableColumn("Level")]
        public int Level { get; set; }
        [ExcelTableColumn("Position")]
        public string Position { get; set; }
        [ExcelTableColumn("Organization Unit")]
        public string OrganizationUnit { get; set; }
        [ExcelTableColumn("Job Purpose")]
        public string JobPurposeEng { get; set; }
        [ExcelTableColumn("Responsibility")]
        public string ResponsibilityEng { get; set; }
        [ExcelTableColumn("KPI")]
        public string KPIEng { get; set; }
    }
}

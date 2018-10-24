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
        [ExcelTableColumn("Job Purpose ENG")]
        public string JobPurposeEng { get; set; }
        [ExcelTableColumn("Responsibility ENG")]
        public string ResponsibilityEng { get; set; }
        [ExcelTableColumn("KPI ENG")]
        public string KPIEng { get; set; }
        [ExcelTableColumn("Job Purpose Local")]
        public string JobPurposeLocal { get; set; }
        [ExcelTableColumn("Responsibility Local")]
        public string ResponsibilityLocal { get; set; }
        [ExcelTableColumn("KPI Local")]
        public string KPILocal{ get; set; }

    }
}

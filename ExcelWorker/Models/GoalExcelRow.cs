using EPPlus.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker.Models
{
    public class GoalExcelRow
    {
        [ExcelTableColumn("Year")]
        public int Year { get; set; }
        [ExcelTableColumn("Country")]
        public string Country { get; set; }
        [ExcelTableColumn("Brand")]
        public string Brand { get; set; }
        [ExcelTableColumn("Position")]
        public string Position { get; set; }
        [ExcelTableColumn("Perspective")]
        public string Perspective { get; set; }
        [ExcelTableColumn("Goals Text")]
        public string Goal { get; set; }
        [ExcelTableColumn("Weight")]
        public int Weight { get; set; }
        [ExcelTableColumn("KPI connected")]
        public string Kpi { get; set; }
    }
}

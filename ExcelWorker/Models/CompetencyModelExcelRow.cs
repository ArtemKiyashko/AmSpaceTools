using EPPlus.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker.Models
{
    public class CompetencyModelExcelRow
    {
        [ExcelTableColumn]
        public int Level { get; set; }

        [ExcelTableColumn]
        public string Brand { get; set; }

        [ExcelTableColumn]
        public string Competency { get; set; }

        [ExcelTableColumn]
        public string Behavior { get; set; }

        [ExcelTableColumn]
        public string Language { get; set; }
    }
}

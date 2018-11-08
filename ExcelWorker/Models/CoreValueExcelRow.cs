using EPPlus.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker.Models
{
    public class CoreValueExcelRow
    {
        [ExcelTableColumn]
        public string Level { get; set; }
        [ExcelTableColumn]
        public string Title { get; set; }
        [ExcelTableColumn]
        public string Description { get; set; }

        [ExcelTableColumn("Lang")]
        public string Language { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.Performance
{
    public class KpiExcelRow
    {
        public int Year { get; set; }
        public string Country { get; set; }
        public string Brand { get; set; }
        public string Position { get; set; }
        public string KpiDescription { get; set; }
        public string KpiTarget { get; set; }
        public string KpiType { get; set; }
    }
}

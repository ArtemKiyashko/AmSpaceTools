using AmSpaceModels.Idp;
using EPPlus.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker.Models
{
    public class IdpExcelRow
    {
        [ExcelTableColumn]
        public string CompetencyName { get; set; }
        [ExcelTableColumn]
        public int CompetencyLevel { get; set; }
        [ExcelTableColumn]
        public string ActionSourceDescription { get; set; }
        [ExcelTableColumn]
        public int ActionPercentage { get; set; }

        [EpplusIgnore]
        public List<Translation> Translations { get; set; }

        [EpplusIgnore]
        public bool Taken { get; set; }
    }
}

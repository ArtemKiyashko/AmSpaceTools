using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels
{
    public class IdpExcelRow
    {
        public string CompetencyName { get; set; }
        public int CompetencyLevel { get; set; }
        public string ActionSourceDescription { get; set; }
        public int ActionPercentage { get; set; }

        [EpplusIgnore]
        public List<Translation> Translations { get; set; }
    }
}

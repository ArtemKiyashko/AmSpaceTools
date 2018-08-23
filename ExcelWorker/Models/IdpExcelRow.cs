﻿using AmSpaceModels.Idp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorker.Models
{
    public class IdpExcelRow
    {
        public string CompetencyName { get; set; }
        public int CompetencyLevel { get; set; }
        public string ActionSourceDescription { get; set; }
        public int ActionPercentage { get; set; }

        [EpplusIgnore]
        public List<Translation> Translations { get; set; }

        [EpplusIgnore]
        public bool Taken { get; set; }
    }
}
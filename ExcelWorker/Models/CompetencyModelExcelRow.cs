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

        [ExcelTableColumn("Competency EN")]
        public string CompetencyEn { get; set; }

        [ExcelTableColumn("Behaviour EN")]
        public string BehaviorEn { get; set; }

        [ExcelTableColumn("Competency PL")]
        public string CompetencyPl { get; set; }

        [ExcelTableColumn("Behaviour PL")]
        public string BehaviorPl { get; set; }

        [ExcelTableColumn("Competency CZ")]
        public string CompetencyCz { get; set; }

        [ExcelTableColumn("Behaviour CZ")]
        public string BehaviorCz { get; set; }

        [ExcelTableColumn("Competency HU")]
        public string CompetencyHu { get; set; }

        [ExcelTableColumn("Behaviour HU")]
        public string BehaviorHu { get; set; }

        [ExcelTableColumn("Competency SK")]
        public string CompetencySk { get; set; }

        [ExcelTableColumn("Behaviour SK")]
        public string BehaviorSk { get; set; }

        [ExcelTableColumn("Competency BG")]
        public string CompetencyBg { get; set; }

        [ExcelTableColumn("Behaviour BG")]
        public string BehaviorBg { get; set; }

        [ExcelTableColumn("Competency DE")]
        public string CompetencyDe { get; set; }

        [ExcelTableColumn("Behaviour DE")]
        public string BehaviorDe { get; set; }

        [ExcelTableColumn("Competency ES")]
        public string CompetencyEs { get; set; }

        [ExcelTableColumn("Behaviour ES")]
        public string BehaviorEs { get; set; }

        [ExcelTableColumn("Competency RO")]
        public string CompetencyRo { get; set; }

        [ExcelTableColumn("Behaviour RO")]
        public string BehaviorRo { get; set; }

        [ExcelTableColumn("Competency RU")]
        public string CompetencyRu { get; set; }

        [ExcelTableColumn("Behaviour RU")]
        public string BehaviorRu { get; set; }

        [ExcelTableColumn("Competency CN")]
        public string CompetencyCn { get; set; }

        [ExcelTableColumn("Behaviour CN")]
        public string BehaviorCn { get; set; }

        [ExcelTableColumn("Competency FR")]
        public string CompetencyFr { get; set; }

        [ExcelTableColumn("Behaviour FR")]
        public string BehaviorFr { get; set; }

        [ExcelTableColumn("Competency HR")]
        public string CompetencyHr { get; set; }

        [ExcelTableColumn("Behaviour HR")]
        public string BehaviorHr { get; set; }

        [ExcelTableColumn("Competency RS")]
        public string CompetencyRs { get; set; }

        [ExcelTableColumn("Behaviour Rs")]
        public string BehaviorRs { get; set; }
    }
}

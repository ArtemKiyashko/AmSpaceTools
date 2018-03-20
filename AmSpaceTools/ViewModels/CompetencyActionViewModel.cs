using AmSpaceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public class CompetencyActionViewModel : BaseViewModel
    {
        public string CompetencyName { get; set; }
        public List<IdpAction> Actions { get; set; }
        public Level CompetencyLevel { get; set; }
        public long TheoryActionsCount { get; set; }
        public long FeedbackActionsCount { get; set; }
        public long PracticeActionsCount { get; set; }
    }
}

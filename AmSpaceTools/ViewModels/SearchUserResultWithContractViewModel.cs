using AmSpaceModels.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public class SearchUserResultWithContractViewModel : BaseViewModel
    {
        public SearchUserResult User { get; set; }
        public ContractSearch MainContract { get; set; }
    }
}

using AmSpaceClient;
using AmSpaceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ViewModels
{
    public class MsWebLoginViewModel : BaseViewModel
    {
        private readonly IAmSpaceClient _client;
        public AmSpaceEnvironment Environment { get; set; }

        public MsWebLoginViewModel(IAmSpaceClient client)
        {
            _client = client;
        }

        public async Task<bool> LoginAsync(AmSpaceEnvironment environment)
        {
            return true;
        }
    }
}

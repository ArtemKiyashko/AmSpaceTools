using AmSpaceModels;
using AmSpaceModels.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceClient
{
    public interface  IAmSpaceSsoClient
    {
        Task<bool> LoginWithCodeFlow(LoginCodeFlowAuthResult model, IAmSpaceEnvironment environment);
    }
}

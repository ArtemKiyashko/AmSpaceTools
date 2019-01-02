using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmSpaceModels;

namespace AmSpaceTools.Infrastructure.Providers
{
    public interface IAmSpaceEnvironmentsProvider
    {
        IEnumerable<AmSpaceEnvironment> Environments { get; }
    }
}

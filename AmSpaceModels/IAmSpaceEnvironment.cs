using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels
{
    public interface IAmSpaceEnvironment
    {
        string Name { get; set; }
        string ClientId { get; set; }
        string BaseAddress { get; set; }
        string GrantPermissionType { get; set; }
    }
}

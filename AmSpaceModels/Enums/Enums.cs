using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.Enums
{
    public enum SapSex
    {
        Male = 1,
        Female = 2
    }

    public enum AmSpaceSex
    {
        Male = 0,
        Female = 1
    }

    public enum AmSpaceUserStatus
    {
        ACTIVE,
        SUSPENDED,
        TERMINATED,
        ANY
    }

    public enum SapUserStatus
    {
        ACTIVE = 3,
        SUSPENDED = 4,
        TERMINATED = 0
    }
}

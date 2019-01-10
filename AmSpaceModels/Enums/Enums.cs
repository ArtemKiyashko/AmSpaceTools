using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.Enums
{
    public enum SapSex
    {
        MALE = 1,
        FEMALE = 2
    }

    public enum AmSpaceSex
    {
        MALE,
        FEMALE
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

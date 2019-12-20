using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels
{
    public interface ICopyable<T>
    {
        T ShallowCopy();
    }
}

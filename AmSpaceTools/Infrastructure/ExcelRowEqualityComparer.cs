using AmSpaceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure
{
    public class ExcelRowEqualityComparer : IEqualityComparer<IdpExcelRow>
    {
        public bool Equals(IdpExcelRow x, IdpExcelRow y)
        {
            return x.CompetencyName == y.CompetencyName &&
                x.CompetencyLevel == y.CompetencyLevel &&
                x.ActionSourceDescription == y.ActionSourceDescription &&
                x.ActionPercentage == y.ActionPercentage;
        }

        public int GetHashCode(IdpExcelRow obj)
        {
            var r = obj.CompetencyName + obj.CompetencyLevel.ToString() + obj.ActionSourceDescription + obj.ActionPercentage.ToString();
            return r.GetHashCode();
        }
    }
}

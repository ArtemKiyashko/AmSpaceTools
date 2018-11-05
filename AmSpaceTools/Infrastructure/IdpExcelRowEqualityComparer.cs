using AmSpaceModels.Idp;
using ExcelWorker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure
{
    public class IdpExcelRowEqualityComparer : IEqualityComparer<IdpExcelRow>
    {
        public bool Equals(IdpExcelRow x, IdpExcelRow y)
        {
            if (ReferenceEquals(x, y)) return true;
            var lowerSourceX = x.CompetencyName.ToLower();
            var lowerSourceY = y.CompetencyName.ToLower();
            return x != null && y != null &&
                (lowerSourceX.Contains(lowerSourceY) || lowerSourceY.Contains(lowerSourceX)) &&
                x.CompetencyLevel.Equals(y.CompetencyLevel) &&
                x.ActionSourceDescription.Equals(y.ActionSourceDescription) &&
                x.ActionPercentage.Equals(y.ActionPercentage);
        }

        public int GetHashCode(IdpExcelRow obj)
        {
            return //obj.CompetencyName.GetHashCode() ^ 
                obj.CompetencyLevel.GetHashCode() ^
                obj.ActionSourceDescription.GetHashCode() ^
                obj.ActionPercentage.GetHashCode();
        }
    }
}

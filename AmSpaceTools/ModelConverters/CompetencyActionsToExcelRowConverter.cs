using AmSpaceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ModelConverters
{
    public class CompetencyActionsToExcelRowConverter : ITypeConverter<IDictionary<Competency, List<IdpAction>>, IEnumerable<IdpExcelRow>>
    {
        public IEnumerable<IdpExcelRow> Convert(IDictionary<Competency, List<IdpAction>> source, IEnumerable<IdpExcelRow> destination, ResolutionContext context)
        {
            var result = new List<IdpExcelRow>();
            foreach (var comp in source)
            {
                foreach (var action in comp.Value)
                {
                    var row = new IdpExcelRow();
                    row.CompetencyName = comp.Key.Name;
                    row.CompetencyLevel = Int32.Parse(comp.Key.Level.Name);
                    row.ActionSourceDescription = action.Name;
                    row.Taken = action.Updated;
                    switch (action.ActionType.Value)
                    {
                        case 0: row.ActionPercentage = 10;
                            break;
                        case 1:
                            row.ActionPercentage = 20;
                            break;
                        case 2:
                            row.ActionPercentage = 70;
                            break;
                    }
                    result.Add(row);
                }
            }
            return result;
        }
    }
}

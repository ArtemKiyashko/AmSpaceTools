using AmSpaceModels;
using AmSpaceTools.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ModelConverters
{
    public class CompetencyActionConverter : ITypeConverter<IEnumerable<IdpExcelRow>, IEnumerable<CompetencyActionDto>>
    {
        public IEnumerable<CompetencyActionDto> Convert(IEnumerable<IdpExcelRow> source, IEnumerable<CompetencyActionDto> destination, ResolutionContext context)
        {
            var result = new List<CompetencyActionDto>();
            foreach (var row in source)
            {
                var model = new CompetencyActionDto
                {
                    CompetencyName = row.CompetencyName,
                    CompetencyLevel = row.CompetencyLevel.ToString(),
                    ActionName = row.ActionSourceDescription,
                    ActionPercentage = row.ActionPercentage
                };
                result.Add(model);
            }
            return result;
        }
    }
}

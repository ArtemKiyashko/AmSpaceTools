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
    public class CompetencyActionConverter : ITypeConverter<IEnumerable<CompetencyActionViewModel>, IEnumerable<CompetencyActionDto>>
    {
        public IEnumerable<CompetencyActionDto> Convert(IEnumerable<CompetencyActionViewModel> source, IEnumerable<CompetencyActionDto> destination, ResolutionContext context)
        {
            var result = new List<CompetencyActionDto>();
            foreach(var competency in source)
            foreach(var action in competency.Actions)
            {
                foreach(var actionTranslation in action.Translations)
                {
                    var model = new CompetencyActionDto
                    {
                        CompetencyName = competency.CompetencyName,
                        CompetencyLevel = competency.CompetencyLevel.Name
                    };
                    model.ActionPercentage = action.ActionType.Value;
                    model.ActionName = action.Name;
                    model.Translation = actionTranslation.Name;
                    model.TranslationLanguage = actionTranslation.Language;
                    result.Add(model);
                }
            }
            return result;
        }
    }
}

using AmSpaceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.ModelConverters
{
    public class IdpExcelRowToUpdateConverter : ITypeConverter<IEnumerable<IdpExcelRow>, IEnumerable<UpdateAction>>
    {
        public IEnumerable<UpdateAction> Convert(IEnumerable<IdpExcelRow> source, IEnumerable<UpdateAction> destination, ResolutionContext context)
        {
            var result = new List<UpdateAction>();
            var sourceGrouped =
                from a in source
                group a by new
                {
                    a.CompetencyName,
                    a.CompetencyLevel
                } into gas
                select new
                {
                    gas.Key.CompetencyName,
                    gas.Key.CompetencyLevel,
                    Result = gas.ToList()
                };
            foreach (var group in sourceGrouped)
            {
                var target = new UpdateAction();
                target.FeedbackActions = new List<IdpAction>();
                target.PracticeActions = new List<IdpAction>();
                target.TheoryActions = new List<IdpAction>();
                foreach (var row in group.Result)
                {
                    var action = new IdpAction();
                    action.Name = row.ActionSourceDescription;
                    action.Translations = new List<Translation>();
                    foreach (var inputTranslation in row.Translations)
                    {
                        action.Translations.Add(new Translation
                        {
                            Language = inputTranslation.Language.ToLower(),
                            Name = inputTranslation.Name
                        });
                    }
                    switch (row.ActionPercentage)
                    {
                        case 0:
                            {
                                target.TheoryActions.Add(action);
                                break;
                            }
                        case 1:
                            {
                                target.FeedbackActions.Add(action);
                                break;
                            }
                        case 2:
                            {
                                target.PracticeActions.Add(action);
                                break;
                            }
                    }
                }
                result.Add(target);
            }
            return result;
        }
    }
}

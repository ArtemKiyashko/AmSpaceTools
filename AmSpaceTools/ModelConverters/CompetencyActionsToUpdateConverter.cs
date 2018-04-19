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
    public class CompetencyActionsToUpdateConverter : ITypeConverter<IEnumerable<CompetencyAction>, IEnumerable<UpdateAction>>
    {
        public IEnumerable<UpdateAction> Convert(IEnumerable<CompetencyAction> source, IEnumerable<UpdateAction> destination, ResolutionContext context)
        {
            var result = new List<UpdateAction>();
            foreach(var competencyAction in source)
            {
                foreach(var action in competencyAction.Actions)
                {
                    var target = new UpdateAction();
                    target.FeedbackActions = new List<IdpAction>();
                    target.PracticeActions = new List<IdpAction>();
                    target.TheoryActions = new List<IdpAction>();
                    switch (action.ActionType.Value)
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
                    result.Add(target);
                }
            }
            return result;
        }
    }
}

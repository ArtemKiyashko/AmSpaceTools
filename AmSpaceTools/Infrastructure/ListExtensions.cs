using AmSpaceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure
{
    public static class ListExtensions
    {
        public static void UpsertTranslation(this IList<Translation> currentTranslations, Translation newTranslation)
        {
            var oldTranslation = currentTranslations.FirstOrDefault(_ => _.Language == newTranslation.Language);
            if (oldTranslation == null)
            {
                currentTranslations.Add(new Translation
                {
                    Language = newTranslation.Language,
                    Name = newTranslation.Name
                });
            }
            else
            {
                oldTranslation.Name = newTranslation.Name;
            }
        }
    }
}

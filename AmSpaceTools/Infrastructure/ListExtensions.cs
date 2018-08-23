using AmSpaceModels;
using AmSpaceModels.Idp;
using ExcelWorker.Models;
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

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static Dictionary<string, List<Translation>> NormalizeTranslations(this IEnumerable<IdpExcelRow> source)
        {
            var uniqueActions = source.ToLookup(x => x.ActionSourceDescription, e => e.Translations);
            var shrinkedDictionary = new Dictionary<string, List<Translation>>();
            foreach (var group in uniqueActions.Where(_ => !string.IsNullOrEmpty(_.Key)))
            {
                var translations = group.SelectMany(_ => _);
                foreach (var translation in translations.Where(_ => !string.IsNullOrEmpty(_.Name)).GroupBy(_ => _.Language))
                {
                    var longestTranslation = translation.Aggregate(new Translation { Name = "" }, (max, cur) => max.Name.Length > cur.Name.Length ? max : cur);
                    if (shrinkedDictionary.ContainsKey(group.Key))
                    {
                        shrinkedDictionary[group.Key].Add(longestTranslation);
                    }
                    else
                    {
                        var list = new List<Translation>();
                        list.Add(longestTranslation);
                        shrinkedDictionary.Add(group.Key, list);
                    }
                }
            }
            return shrinkedDictionary;
        }
    }
}

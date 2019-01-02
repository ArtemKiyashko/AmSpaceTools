using F23.StringSimilarity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue UpsertKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
        {
            TValue currentValue;
            if (!dict.TryGetValue(key, out currentValue))
                dict.Add(key, new TValue());
            return dict[key];
        }

        public static KeyValuePair<string, TValue> FindSimilar<TValue>(this IDictionary<string, TValue> dict, string key, double similarityPercent)
        {
            if (similarityPercent == 100)
                return new KeyValuePair<string, TValue>(key, dict.ContainsKey(key) ? dict[key] : default);

            var cosine = new Cosine();
            var similarityValue = similarityPercent / 100;
            var similarityDictionary = new List<KeyValuePair<double, KeyValuePair<string, TValue>>>();
            foreach (var val in dict)
                similarityDictionary.Add(new KeyValuePair<double, KeyValuePair<string, TValue>>(cosine.Similarity(key, val.Key), val));

            return similarityDictionary
                    .Where(_ => _.Key >= similarityValue)
                    .OrderByDescending(_ => _.Key)
                    .FirstOrDefault()
                    .Value;
        }

        public static Task<KeyValuePair<string, TValue>> FindSimilarAsync<TValue>(this IDictionary<string, TValue> dict, string key, double similarityPercent)
        {
            return Task.Run(() => FindSimilar(dict, key, similarityPercent));
        }
    }
}

using F23.StringSimilarity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure
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

        public static KeyValuePair<TKey, TValue> FindSimilar<TKey, TValue>(this IDictionary<TKey, TValue> dict, string key, double similarityPercent)
        {
            var cosine = new Cosine();
            var similarityValue = similarityPercent / 100;
            var similarityDictionary = new List<KeyValuePair<double, KeyValuePair<TKey, TValue>>>();
            foreach (var val in dict)
                similarityDictionary.Add(new KeyValuePair<double, KeyValuePair<TKey, TValue>>(cosine.Similarity(key, val.Key.ToString()), val));

            return similarityDictionary
                    .Where(_ => _.Key >= similarityValue)
                    .OrderByDescending(_ => _.Key)
                    .FirstOrDefault()
                    .Value;
    }
    }
}

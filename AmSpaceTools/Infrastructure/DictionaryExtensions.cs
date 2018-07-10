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
    }
}

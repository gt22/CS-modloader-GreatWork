using System;
using System.Collections.Generic;
using HarmonyLib;

namespace GreatWorkIvory.Utils
{
    public static class DictUtils
    {

        public static TValue ComputeIfAbsent<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key,
            Func<TKey, TValue> comp)
        {
            if (d.TryGetValue(key, out var x))
            {
                return x;
            }
            return d[key] = comp(key);
        }

        public static TValue Compute<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key,
            Func<TKey, TValue, TValue> comp)
        {
            return d[key] = comp(key, d.GetValueSafe(key));
        }
    }
}
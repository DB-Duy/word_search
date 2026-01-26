using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using UnityEngine;

namespace Shared.Utils
{
    public static class LinqUtility
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
        
        public static void AddRange(this Dictionary<string, string> collection, string[] items)
        {
            for (var i = 1; i < items.Length; i += 2)
            {
                collection.Upsert(items[i -1], items[i]);
            }
        }
        
        public static void AddRange(this Dictionary<string, string> collection, Dictionary<string, string> items)
        {
            foreach (var kv in items) collection.Upsert(kv.Key, kv.Value);
        }
        
        public static void Upsert(this IDictionary<string, object> dict, string name, object value)
        {
            if (dict.ContainsKey(name))
            {
                dict[name] = value;
                return;
            }

            dict.Add(name, value);
        }
        
        public static bool UpsertWithResult(this IDictionary<string, object> dict, string name, object value)
        {
            if (dict.ContainsKey(name))
            {
                if (value == dict[name]) return false;
                dict[name] = value;
                return true;
            }

            dict.Add(name, value);
            return true;
        }
        
        public static void UpsertPrimaryValue(this IDictionary<string, object> dict, string name, object unidentifiedValue)
        {
            var value = unidentifiedValue;
            if (unidentifiedValue is ValueObject valueObject) value = valueObject.Value;
            if (dict.ContainsKey(name))
            {
                dict[name] = value;
                return;
            }

            dict.Add(name, value);
        }
        
        public static void Upsert<T>(this IDictionary<string, T> dict, string name, T value)
        {
            if (dict.ContainsKey(name))
            {
                dict[name] = value;
                return;
            }

            dict.Add(name, value);
        }
        
        public static void Upsert(this IDictionary<string, object> dict, IDictionary<string, object> otherDict)
        {
            foreach (var kv in otherDict)
            {
                if (dict.ContainsKey(kv.Key)) dict[kv.Key] = kv.Value; else dict.Add(kv.Key, kv.Value);
            }
        }
        
        public static bool AddIfNotExisted<T>(this IDictionary<string, T> dict, string name, T value)
        {
            if (dict.ContainsKey(name)) return false;
            dict.Add(name, value);
            return true;
        }
        
        public static object Get(this IDictionary<string, object> dict, string key, object defaultData) => !dict.ContainsKey(key) ? defaultData : dict[key];
        public static string Get(this Dictionary<string, string> m, string key, string defaultValue = null) => m.ContainsKey(key) ? m[key] : defaultValue;
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultData) => dict == null ? defaultData : dict.TryGetValue(key, out TValue value) ? value : defaultData;
        
        public static Dictionary<string, string> Convert(this IDictionary<string, object> dict)
        {
            Dictionary<string, string> resultDict = new();
            foreach (var kv in dict)
            {
                if (kv.Value == null) resultDict.Add(kv.Key, null);
                else resultDict.Add(kv.Key, kv.Value.ToString());
            }

            return resultDict;
        }
        
        public static IDictionary<string, string> ToDictStringString(this IDictionary<string, object> dict)
        {
            return dict.ToDictionary(e => e.Key, e => e.Value == null ? "null" : e.Value.ToString());
        }
        
        private static readonly System.Random Rng = new();
        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        
        public static string[] SplitIntoChunks(this string source, int chunkSize)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (chunkSize <= 0) throw new ArgumentException("Chunk size must be greater than zero.", nameof(chunkSize));

            var chunks = new List<string>();
            for (var i = 0; i < source.Length; i += chunkSize)
            {
                if (i + chunkSize > source.Length)
                    chunks.Add(source.Substring(i));
                else
                    chunks.Add(source.Substring(i, chunkSize));
            }

            return chunks.ToArray();
        }
        
        public static string[] SplitBy(this string source, string sep)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (sep == null) throw new ArgumentException(nameof(sep));
            var result = sep.Split(sep);

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = result[i].Replace(" ", "");
            }

            return result;
        }

        public static float CastToFloat(this string src)
        {
            return float.Parse(src.Trim(), CultureInfo.InvariantCulture);
        }
        
        public static float CastToFloat(this string src, float defaultValue)
        {
            try
            {
                return float.Parse(src.Trim(), CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Debug.LogError($"LinqUtility->CastFrom: src={src}, defaultValue={defaultValue}, e={e.Message}");
                Debug.LogException(e);
                return defaultValue;
            }
        }

        public static string ToJsonString(this object obj) => JsonConvert.SerializeObject(obj);
    }
}
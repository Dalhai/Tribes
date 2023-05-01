using System;
using System.Collections.Generic;

namespace TribesOfDust.Utils.Extensions
{
    public static class DictionaryExtensions
    {
        #region Get or Add

        public static TValue GetOr<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue or) => dictionary.ContainsKey(key) ? dictionary[key] : or;
        public static TValue GetOr<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> or) => dictionary.ContainsKey(key) ? dictionary[key] : or();

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue add)
        {
            if (!dictionary.ContainsKey(key))
                dictionary.Add(key, add);

            return dictionary[key];
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> add)
        {
            if (!dictionary.ContainsKey(key))
                dictionary.Add(key, add());

            return dictionary[key];
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> add)
        {
            if (!dictionary.ContainsKey(key))
                dictionary.Add(key, add(key));

            return dictionary[key];
        }

        #endregion
        #region Update

        public static void Update<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TValue, TValue> update)
        {
            foreach (var kvp in dictionary)
            {
                dictionary[kvp.Key] = update(kvp.Value);
            }
        }

        public static void Update<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue, TValue> update)
        {
            foreach (var kvp in dictionary)
            {
                dictionary[kvp.Key] = update(kvp.Key, kvp.Value);
            }
        }

        public static void Update<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> update)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = update(dictionary[key]);
        }

        public static void Update<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue, TValue> update)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = update(key, dictionary[key]);
        }

        #endregion
        #region Update or Add

        public static void UpdateOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> update, TValue add)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = add;
            }
            else
            {
                dictionary[key] = update(dictionary[key]);
            }
        }

        public static void UpdateOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> update, Func<TValue> add)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = add();
            }
            else
            {
                dictionary[key] = update(dictionary[key]);
            }
        }

        public static void UpdateOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> update, Func<TKey, TValue> add)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = add(key);
            }
            else
            {
                dictionary[key] = update(dictionary[key]);
            }
        }

        public static void UpdateOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue, TValue> update, TValue add)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = add;
            }
            else
            {
                dictionary[key] = update(key, dictionary[key]);
            }
        }

        public static void UpdateOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue, TValue> update, Func<TValue> add)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = add();
            }
            else
            {
                dictionary[key] = update(key, dictionary[key]);
            }
        }

        public static void UpdateOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue, TValue> update, Func<TKey, TValue> add)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = add(key);
            }
            else
            {
                dictionary[key] = update(key, dictionary[key]);
            }
        }

        #endregion
    }
}
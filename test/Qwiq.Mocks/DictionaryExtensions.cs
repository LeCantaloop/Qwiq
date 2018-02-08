using System.Collections.Generic;

namespace Qwiq.Mocks
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Tries to add a key to the dictionary, if it does not already exist.
        /// </summary>
        /// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2" /> instance where <c>TValue</c> is <c>object</c></param>
        /// <param name="key">The key to add</param>
        /// <param name="value">The value to add</param>
        /// <returns><c>true</c> if the key was added with the specified value. If the key already exists, the method returns <c>false</c> without updating the value.</returns>
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                return false;
            dictionary.Add(key, value);
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Identity
{
    public abstract class IdentityValueConverterBase : IIdentityValueConverter<string, object>
    {
        public virtual object Map(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            var r = Map(new[] {value});
            if (r != null)
            {
                var kvp = r.FirstOrDefault();
                if (!EqualityComparer<KeyValuePair<string, object>>.Default.Equals(kvp, default(KeyValuePair<string, object>)))
                {
                    if (kvp.Value != null)
                    {
                        return kvp.Value;
                    }
                }
            }

            return value;
        }
        public abstract IReadOnlyDictionary<string, object> Map(IEnumerable<string> values);

        [Obsolete("This method is depreciated and will be removed in a future version.")]
        public virtual object Map(object value)
        {
            if (value is string stringValue) return Map(stringValue);
            if (value is IEnumerable<string> stringArray)
            {
                return Map(stringArray).Select(s=>s.Value).ToArray();
            }

            return value;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    public static partial class Extensions
    {
        public static IWorkItem NewWorkItem(this IWorkItemType wit, IEnumerable<KeyValuePair<string, object>> values)
        {
            if (wit == null) throw new ArgumentNullException(nameof(wit));
            var wi = wit.NewWorkItem();

            if (values == null)
            {
                return wi;
            }
            foreach (var kvp in values) wi[kvp.Key] = kvp.Value;

            return wi;
        }

        public static IEnumerable<IWorkItem> NewWorkItems(
            this IWorkItemType wit,
            IEnumerable<IEnumerable<KeyValuePair<string, object>>> values)
        {
            return values.Select(wit.NewWorkItem);
        }
    }
}

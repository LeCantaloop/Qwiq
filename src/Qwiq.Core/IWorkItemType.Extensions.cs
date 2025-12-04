using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Qwiq
{
    public static partial class Extensions
    {
        public static IWorkItem NewWorkItem(this IWorkItemType wit, IEnumerable<KeyValuePair<string, object>> values)
        {
            Contract.Requires(wit != null);

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
            Contract.Requires(values != null);
            Contract.Requires(wit != null);

            if (wit == null) throw new ArgumentNullException(nameof(wit));
            if (values == null) throw new ArgumentNullException(nameof(values));

            return values.Select(wit.NewWorkItem);
        }
    }
}
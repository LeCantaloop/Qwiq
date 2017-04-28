using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public static partial class Extensions
    {
        [MustUseReturnValue]
        [ContractAnnotation("wit:null => halt")]
        public static IWorkItem NewWorkItem([NotNull] this IWorkItemType wit, [CanBeNull] IEnumerable<KeyValuePair<string, object>> values)
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

        [ContractAnnotation("wit:null => halt")]
        public static IEnumerable<IWorkItem> NewWorkItems(
            [NotNull] this IWorkItemType wit,
            [InstantHandle] [NotNull] IEnumerable<IEnumerable<KeyValuePair<string, object>>> values)
        {
            Contract.Requires(values != null);
            Contract.Requires(wit != null);

            if (wit == null) throw new ArgumentNullException(nameof(wit));
            if (values == null) throw new ArgumentNullException(nameof(values));


            return values.Select(wit.NewWorkItem);
        }
    }
}

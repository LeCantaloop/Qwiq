using System;

namespace Microsoft.Qwiq
{
    public static partial class Extensions
    {
        public static void AddRelatedLink(this IWorkItem workItem, IWorkItemStore store, int[] targets)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (targets.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(targets));

            var end = store.WorkItemLinkTypes[CoreLinkTypeReferenceNames.Related].ForwardEnd;

            foreach (var id in targets) workItem.Links.Add(workItem.CreateRelatedLink(id, end));
        }

       

        
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    public static class WorkItemExtensions
    {
        public static void AddRelatedLink(this IWorkItem workItem, IWorkItemStore store, int[] targets)
        {
            var end = store.WorkItemLinkTypes[CoreLinkTypeReferenceNames.Related].ForwardEnd;

            foreach (var id in targets) workItem.Links.Add(workItem.CreateRelatedLink(id, end));
        }

        public static IWorkItemLinkTypeEnd GetChildLinkTypeEnd(this IWorkItemStore store)
        {
            return store.GetLinkType(CoreLinkTypeReferenceNames.Hierarchy).ReverseEnd;
        }

        public static IWorkItemLinkType GetLinkType(this IWorkItemStore store, string linkTypeReferenceName)
        {
            return store.WorkItemLinkTypes[linkTypeReferenceName];
        }

        public static IWorkItemLinkTypeEnd GetParentLinkTypeEnd(this IWorkItemStore store)
        {
            return store.GetChildLinkTypeEnd().OppositeEnd;
        }

        public static IWorkItem NewWorkItem(this IWorkItemType wit, IEnumerable<KeyValuePair<string, object>> values)
        {
            var wi = wit.NewWorkItem();

            if (values != null) foreach (var kvp in values) wi[kvp.Key] = kvp.Value;

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
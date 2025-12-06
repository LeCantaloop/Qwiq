using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;


namespace Qwiq
{
    public static partial class Extensions
    {
        public static void AddRelatedLink(this IWorkItem workItem, IWorkItemStore store, int targetId)
        {
            Contract.Requires(workItem != null);
            Contract.Requires(store != null);
            Contract.Requires(targetId > 0);

            ArgumentNullException.ThrowIfNull(workItem);
            ArgumentNullException.ThrowIfNull(store);
            ArgumentOutOfRangeException.ThrowIfZero(targetId);

            var end = store.WorkItemLinkTypes[CoreLinkTypeReferenceNames.Related].ForwardEnd;
            workItem.Links.Add(workItem.CreateRelatedLink(targetId, end));
        }
        public static void AddParentLink(this IWorkItem workItem, IWorkItemStore store, int parentId)
        {
            Contract.Requires(workItem != null);
            Contract.Requires(store != null);
            Contract.Requires(parentId > 0);

            ArgumentNullException.ThrowIfNull(workItem);
            ArgumentNullException.ThrowIfNull(store);
            ArgumentOutOfRangeException.ThrowIfZero(parentId);

            var end = store.GetParentLinkTypeEnd();
            workItem.Links.Add(workItem.CreateRelatedLink(parentId, end));
        }
        public static void AddChildLink(this IWorkItem workItem, IWorkItemStore store, int childId)
        {
            Contract.Requires(workItem != null);
            Contract.Requires(store != null);
            Contract.Requires(childId > 0);

            ArgumentNullException.ThrowIfNull(workItem);
            ArgumentNullException.ThrowIfNull(store);
            ArgumentOutOfRangeException.ThrowIfZero(childId);

            var end = store.GetChildLinkTypeEnd();
            workItem.Links.Add(workItem.CreateRelatedLink(childId, end));
        }
        public static void AddChildrenLink(this IWorkItem workItem, IWorkItemStore store, params int[] childrenIds)
        {
            ArgumentNullException.ThrowIfNull(workItem);
            ArgumentNullException.ThrowIfNull(store);
            ArgumentNullException.ThrowIfNull(childrenIds);
            if (childrenIds.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(childrenIds));

            var end = store.GetChildLinkTypeEnd();
            foreach (var id in childrenIds) workItem.Links.Add(workItem.CreateRelatedLink(id, end));
        }
        public static void AddRelatedLink(this IWorkItem workItem, IWorkItemStore store, int[] targets)
        {
            ArgumentNullException.ThrowIfNull(workItem);
            ArgumentNullException.ThrowIfNull(store);
            if (targets.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(targets));

            var end = store.WorkItemLinkTypes[CoreLinkTypeReferenceNames.Related].ForwardEnd;

            foreach (var id in targets) workItem.Links.Add(workItem.CreateRelatedLink(id, end));
        }
        public static IWorkItemCollection ToWorkItemCollection(this IEnumerable<IWorkItem> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            if (items is IWorkItemCollection items2) return items2;

            return new WorkItemCollection(items.Distinct(Comparer.WorkItem).ToList());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

namespace Qwiq
{
    public static partial class Extensions
    {
        [PublicAPI]
        public static void AddRelatedLink([NotNull] this IWorkItem workItem, [NotNull] IWorkItemStore store, int targetId)
        {
            Contract.Requires(workItem != null);
            Contract.Requires(store != null);
            Contract.Requires(targetId > 0);

            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (targetId == 0) throw new ArgumentOutOfRangeException(nameof(targetId));

            var end = store.WorkItemLinkTypes[CoreLinkTypeReferenceNames.Related].ForwardEnd;
            workItem.Links.Add(workItem.CreateRelatedLink(targetId, end));
        }

        [PublicAPI]
        public static void AddParentLink([NotNull] this IWorkItem workItem, [NotNull] IWorkItemStore store, int parentId)
        {
            Contract.Requires(workItem != null);
            Contract.Requires(store != null);
            Contract.Requires(parentId > 0);

            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (parentId == 0) throw new ArgumentOutOfRangeException(nameof(parentId));

            var end = store.GetParentLinkTypeEnd();
            workItem.Links.Add(workItem.CreateRelatedLink(parentId, end));
        }

        [PublicAPI]
        public static void AddChildLink([NotNull] this IWorkItem workItem, [NotNull] IWorkItemStore store, int childId)
        {
            Contract.Requires(workItem != null);
            Contract.Requires(store != null);
            Contract.Requires(childId > 0);

            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (childId == 0) throw new ArgumentOutOfRangeException(nameof(childId));

            var end = store.GetChildLinkTypeEnd();
            workItem.Links.Add(workItem.CreateRelatedLink(childId, end));
        }

        [PublicAPI]
        public static void AddChildrenLink([NotNull] this IWorkItem workItem, [NotNull] IWorkItemStore store, [NotNull] params int[] childrenIds)
        {
            Contract.Requires(workItem != null);
            Contract.Requires(store != null);
            Contract.Requires(childrenIds != null);
            Contract.Requires(childrenIds.Length > 0);

            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (childrenIds == null) throw new ArgumentNullException(nameof(childrenIds));
            if (childrenIds.Length == 0) throw new ArgumentNullException(nameof(childrenIds));

            var end = store.GetChildLinkTypeEnd();
            foreach(var id in childrenIds) workItem.Links.Add(workItem.CreateRelatedLink(id, end));
        }

        [PublicAPI]
        public static void AddRelatedLink(this IWorkItem workItem, IWorkItemStore store, int[] targets)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (targets.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(targets));

            var end = store.WorkItemLinkTypes[CoreLinkTypeReferenceNames.Related].ForwardEnd;

            foreach (var id in targets) workItem.Links.Add(workItem.CreateRelatedLink(id, end));
        }

        [PublicAPI]
        public static IWorkItemCollection ToWorkItemCollection([NotNull] [ItemNotNull] [InstantHandle] this IEnumerable<IWorkItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (items is IWorkItemCollection items2) return items2;

            return new WorkItemCollection(items.Distinct(Comparer.WorkItem).ToList());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace Qwiq.Mocks
{
    public static partial class Extensions
    {
        public static MockWorkItemStore Add(this MockWorkItemStore store, IEnumerable<IWorkItem> workItems)
        {
            return store.Add(workItems, null);
        }

        public static MockWorkItemStore Add(this MockWorkItemStore store, params IWorkItem[] workItems)
        {
            return Add(store, workItems as IEnumerable<IWorkItem>);
        }

        public static MockWorkItemStore Add(this MockWorkItemStore store, IEnumerable<KeyValuePair<string, object>> values)
        {
            Create(store, values);
            return store;
        }

        public static MockWorkItemStore Add(this MockWorkItemStore store, IEnumerable<IEnumerable<KeyValuePair<string, object>>> values)
        {
            foreach (var value in values) store.Add(value);
            return store;
        }

        public static MockWorkItemStore Add(
            this MockWorkItemStore store,
            IEnumerable<IWorkItem> workItems,
            IEnumerable<IWorkItemLinkInfo> links)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (workItems == null && links == null)
                throw new ArgumentException($"Both {nameof(workItems)} and {nameof(links)} cannot be null.");
            if (links != null && workItems == null)
                throw new ArgumentNullException(nameof(workItems), $"{nameof(workItems)} cannot be null when {nameof(links)} is not null.");

            if (workItems != null) store.BatchSave(workItems);

            if (links != null && workItems != null)
            {
                var wi = workItems.ToDictionary(k => k.Id, e => e);
                foreach (var link in links.OrderBy(p => p.SourceId))
                {
                    if (link.SourceId == 0)
                    {
                        var w = wi[link.TargetId];
                        w.Links.Add(w.CreateRelatedLink(0));
                        store.BatchSave(w);
                    }
                    else
                    {
                        var w = wi[link.SourceId];
                        w.Links.Add(w.CreateRelatedLink(link.TargetId, link.LinkType));
                        store.BatchSave(w);
                    }
                }
            }

            return store;
        }

        public static MockWorkItemStore AddLink(this MockWorkItemStore store, int targetId, int sourceId, string linkType)
        {
            var child = store.Query(targetId);

            if (child == null)

                throw new ArgumentException($"Parameter {nameof(targetId)} ({targetId}) does not refer to a work item in the store.");

            var lt = store.WorkItemLinkTypes[linkType];

            child.Links.Add(child.CreateRelatedLink(sourceId, lt.ForwardEnd));

            store.BatchSave(child);


            return store;
        }

        public static MockWorkItemStore AddChildLink(this MockWorkItemStore store, int parentId, int childId)
        {
            var child = store.Query(childId);
            if (child == null)
                throw new ArgumentException($"Parameter {nameof(childId)} ({childId}) does not refer to a work item in the store.");

            child.Links.Add(child.CreateRelatedLink(parentId, store.GetChildLinkTypeEnd()));
            store.BatchSave(child);

            return store;
        }

        public static MockWorkItem Create(this MockWorkItemStore store)
        {
            return Create(store, null);
        }

        public static MockWorkItem Create(this MockWorkItemStore store, IEnumerable<KeyValuePair<string, object>> values = null)
        {
            var project = store.Projects[0];
            var wit = project.WorkItemTypes[0];

            var tp = new KeyValuePair<string, object>(CoreFieldRefNames.TeamProject, project.Name);
            var wp = new KeyValuePair<string, object>(CoreFieldRefNames.WorkItemType, wit.Name);
            var a = new[] { tp, wp };

            values = values?.Union(a) ?? a;

            var wi = (MockWorkItem)wit.NewWorkItem(values);
            store.BatchSave(wi);
            return wi;
        }

        public static MockWorkItem Generate(this MockWorkItemStore store)
        {
            var g = new WorkItemGenerator<MockWorkItem>(store.Create, new[] { "Revisions", "Item" });
            return g.Generate(1).Single();
        }

        [CanBeNull]
        public static IWorkItemStore Store([CanBeNull] this IWorkItemType type)
        {
            var t = type as MockWorkItemType;
            return t?.Store;
        }

        public static MockWorkItemStore WithLinkType(this MockWorkItemStore store, params IWorkItemLinkType[] linkTypes)
        {
            store.WorkItemLinkTypes = new WorkItemLinkTypeCollection(store.WorkItemLinkTypes.Union(linkTypes).ToList());
            return store;
        }

        internal static void BatchSave(this MockWorkItemStore store, params IWorkItem[] workItems)
        {
            store.BatchSave(workItems);
        }
    }
}
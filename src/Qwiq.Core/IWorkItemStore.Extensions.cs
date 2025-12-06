using System;

namespace Qwiq
{
    public static partial class Extensions
    {
        public static IWorkItemLinkTypeEnd GetChildLinkTypeEnd(this IWorkItemStore store)
        {
            ArgumentNullException.ThrowIfNull(store);
            return store.GetLinkType(CoreLinkTypeReferenceNames.Hierarchy).ReverseEnd;
        }

        public static IWorkItemLinkType GetLinkType(this IWorkItemStore store, string linkTypeReferenceName)
        {
            ArgumentNullException.ThrowIfNull(store);
            return store.WorkItemLinkTypes[linkTypeReferenceName];
        }

        public static IWorkItemLinkTypeEnd GetParentLinkTypeEnd(this IWorkItemStore store)
        {
            return store.GetChildLinkTypeEnd().OppositeEnd;
        }
    }
}
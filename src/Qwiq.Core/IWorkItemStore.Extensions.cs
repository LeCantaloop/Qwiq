using System;

namespace Microsoft.Qwiq
{
    public static partial class Extensions
    {
        public static IWorkItemLinkTypeEnd GetChildLinkTypeEnd(this IWorkItemStore store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            return store.GetLinkType(CoreLinkTypeReferenceNames.Hierarchy).ReverseEnd;
        }

        public static IWorkItemLinkType GetLinkType(this IWorkItemStore store, string linkTypeReferenceName)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            return store.WorkItemLinkTypes[linkTypeReferenceName];
        }

        public static IWorkItemLinkTypeEnd GetParentLinkTypeEnd(this IWorkItemStore store)
        {
            return store.GetChildLinkTypeEnd().OppositeEnd;
        }

       
    }
}

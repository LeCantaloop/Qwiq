using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public static class Comparer
    {
        public static IEqualityComparer<IReadOnlyObjectWithIdList<IField, int>> FieldCollection =
                ReadOnlyCollectionWithIdComparer<IField, int>.Default;

        public static IEqualityComparer<IFieldDefinitionCollection> FieldDefinitionCollection = FieldDefinitionCollectionComparer.Default;

        public static IEqualityComparer<IIdentifiable<int>> Identifiable = IdentifiableComparer.Default;

        public static IEqualityComparer<IIdentityDescriptor> IdentityDescriptor = IdentityDescriptorComparer.Default;

        public static IEqualityComparer<IReadOnlyObjectWithIdList<INode, int>> NodeCollection =
                ReadOnlyCollectionWithIdComparer<INode, int>.Default;

        public static IEqualityComparer<string> OrdinalIgnoreCase = StringComparer.OrdinalIgnoreCase;

        public static IEqualityComparer<IWorkItem> WorkItem = WorkItemComparer.Default;

        public static IEqualityComparer<IWorkItemLinkTypeEnd> WorkItemLinkTypeEnd = WorkItemLinkTypeEndComparer.Default;

        public static IEqualityComparer<IWorkItemCollection> WorkItemCollection = WorkItemCollectionComparer.Default;
    }
}
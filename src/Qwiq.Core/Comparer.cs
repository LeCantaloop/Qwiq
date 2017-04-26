using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public static class Comparer
    {
        public static IEqualityComparer<IReadOnlyObjectWithIdList<IField, int>> FieldCollection { get; } =
            ReadOnlyCollectionWithIdComparer<IField, int>.Default;

        public static IEqualityComparer<IFieldDefinition> FieldDefinition { get; } = FieldDefinitionComparer.Default;

        public static IEqualityComparer<IFieldDefinitionCollection> FieldDefinitionCollection { get; } =
            FieldDefinitionCollectionComparer.Default;

        public static IEqualityComparer<IIdentifiable<int>> Identifiable { get; } = IdentifiableComparer.Default;

        public static IEqualityComparer<IIdentityDescriptor> IdentityDescriptor { get; } = IdentityDescriptorComparer.Default;

        public static IEqualityComparer<INode> Node { get; } = NodeComparer.Default;

        public static IEqualityComparer<IReadOnlyObjectWithIdList<INode, int>> NodeCollection { get; } =
            ReadOnlyCollectionWithIdComparer<INode, int>.Default;

        public static IEqualityComparer<IIdentifiable<int?>> NullableIdentity { get; } = NullableIdentifiableComparer.Default;

        public static IEqualityComparer<string> OrdinalIgnoreCase { get; } = StringComparer.OrdinalIgnoreCase;

        public static IEqualityComparer<IProject> Project { get; } = ProjectComparer.Default;

        public static IEqualityComparer<ITeamFoundationIdentity> TeamFoundationIdentity { get; } = TeamFoundationIdentityComparer.Default;

        public static IEqualityComparer<IWorkItem> WorkItem { get; } = WorkItemComparer.Default;

        public static IEqualityComparer<IWorkItemCollection> WorkItemCollection { get; } = WorkItemCollectionComparer.Default;

        public static IEqualityComparer<IWorkItemLinkInfo> WorkItemLinkInfo { get; } = WorkItemLinkInfoComparer.Default;

        public static IEqualityComparer<IWorkItemLinkType> WorkItemLinkType { get; } = WorkItemLinkTypeComparer.Default;

        public static IEqualityComparer<IWorkItemLinkTypeEnd> WorkItemLinkTypeEnd { get; } = WorkItemLinkTypeEndComparer.Default;

        public static IEqualityComparer<IWorkItemType> WorkItemType { get; } = WorkItemTypeComparer.Default;

        public static IEqualityComparer<IWorkItemTypeCollection> WorkItemTypeCollection { get; } = WorkItemTypeCollectionComparer.Default;
    }
}
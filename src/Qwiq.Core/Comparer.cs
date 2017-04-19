using System;

namespace Microsoft.Qwiq
{
    public static class Comparer
    {
        public static readonly ReadOnlyCollectionWithIdComparer<IField, int> FieldCollection =
                ReadOnlyCollectionWithIdComparer<IField, int>.Default;

        public static readonly FieldDefinitionCollectionComparer FieldDefinitionCollection = FieldDefinitionCollectionComparer.Default;

        public static readonly IdentifiableComparer Identifiable = IdentifiableComparer.Default;

        public static readonly IdentityDescriptorComparer IdentityDescriptor = IdentityDescriptorComparer.Default;

        public static readonly ReadOnlyCollectionWithIdComparer<INode, int> NodeCollection =
                ReadOnlyCollectionWithIdComparer<INode, int>.Default;

        public static readonly WorkItemLinkTypeEndComparer WorkItemLinkTypeEnd = WorkItemLinkTypeEndComparer.Default;
        public static readonly StringComparer OrdinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
    }
}
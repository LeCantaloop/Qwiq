using System;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    internal class FieldDefinitionComparer : GenericComparer<IFieldDefinition>
    {
        private FieldDefinitionComparer()
        {

        }

        [NotNull]
        internal new static FieldDefinitionComparer Default => Nested.Instance;

        public override bool Equals([CanBeNull] IFieldDefinition x, [CanBeNull] IFieldDefinition y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            // Not including Id because the int value only appears in SOAP responses

            return StringComparer.OrdinalIgnoreCase.Equals(x.Name, y.Name)
                && StringComparer.OrdinalIgnoreCase.Equals(x.ReferenceName, y.ReferenceName);
        }

        public override int GetHashCode([CanBeNull] IFieldDefinition obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            unchecked
            {
                var hash = 27;
                hash = (13 * hash) ^ (obj.Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name) : 0);
                hash = (13 * hash) ^ (obj.ReferenceName != null
                                          ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ReferenceName)
                                          : 0);

                return hash;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        private class Nested
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly FieldDefinitionComparer Instance = new FieldDefinitionComparer();

            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
            static Nested()
            {
            }
        }
    }
}
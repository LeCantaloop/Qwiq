using System;

namespace Microsoft.Qwiq
{
    public class FieldDefinitionComparer : GenericComparer<IFieldDefinition>
    {
        public static FieldDefinitionComparer Instance => Nested.Instance;

        public override bool Equals(IFieldDefinition x, IFieldDefinition y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(x.ReferenceName, y.ReferenceName, StringComparison.OrdinalIgnoreCase);
        }

        private class Nested
        {
            internal static readonly FieldDefinitionComparer Instance = new FieldDefinitionComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}
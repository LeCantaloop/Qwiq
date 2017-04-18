using System;

namespace Microsoft.Qwiq
{
    public class WorkItemTypeComparer : GenericComparer<IWorkItemType>
    {
        public static WorkItemTypeComparer Instance => Nested.Instance;

        public override bool Equals(IWorkItemType x, IWorkItemType y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(x.Description, y.Description, StringComparison.OrdinalIgnoreCase)
                   && FieldDefinitionCollectionComparer.Instance.Equals(x.FieldDefinitions, y.FieldDefinitions);
        }

        public override int GetHashCode(IWorkItemType obj)
        {
            // Disable overflow compiler check
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) ^ (obj.Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name) : 0);
                hash = (13 * hash) ^ (obj.Description != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Description) : 0);
                hash = (13 * hash) ^ (obj.FieldDefinitions != null ? obj.FieldDefinitions.GetHashCode() : 0);

                return hash;
            }
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class Nested
            // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly WorkItemTypeComparer Instance = new WorkItemTypeComparer();

            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}
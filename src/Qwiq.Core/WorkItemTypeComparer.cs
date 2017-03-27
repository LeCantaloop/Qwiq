using System;
using System.Linq;

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
                   && x.FieldDefinitions.All(p => y.FieldDefinitions.Contains(p, FieldDefinitionComparer.Instance));
        }

        public override int GetHashCode(IWorkItemType obj)
        {
            unchecked
            {
                return 397 * (obj.Name.GetHashCode() ^ obj.Description.GetHashCode());
            }
        }

        private class Nested
        {
            internal static readonly WorkItemTypeComparer Instance = new WorkItemTypeComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}
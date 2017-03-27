using System;

namespace Microsoft.Qwiq.Proxies
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
                && string.Equals(x.Description,y.Description,StringComparison.OrdinalIgnoreCase)
                && x.FieldDefinitions.Equals(y.FieldDefinitions);
        }

        public override int GetHashCode(IWorkItemType obj)
        {
            // Disable overflow compiler check
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) ^ (obj.Name != null ? obj.Name.GetHashCode() : 0);
                hash = (13 * hash) ^ (obj.Description != null ? obj.Description.GetHashCode() : 0);
                hash = (13 * hash) ^ (obj.FieldDefinitions != null ? obj.FieldDefinitions.GetHashCode() : 0);

                return hash;
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
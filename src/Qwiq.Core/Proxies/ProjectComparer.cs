using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Proxies
{
    public class ProjectComparer : GenericComparer<IProject>
    {
        public static ProjectComparer Instance => Nested.Instance;

        public override bool Equals(IProject x, IProject y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)
                && x.Guid.Equals(y.Guid)
                && Equals(x.AreaRootNodes, y.AreaRootNodes)
                && Equals(x.IterationRootNodes, y.IterationRootNodes)
                && x.WorkItemTypes.All(p => y.WorkItemTypes.Contains(p, WorkItemTypeComparer.Instance));
        }

        public override int GetHashCode(IProject obj)
        {
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) ^ obj.Guid.GetHashCode();
                hash = (13 * hash) ^ (obj.Name != null ? obj.Name.GetHashCode() : 0);
                hash = (13 * hash) ^ (obj.AreaRootNodes != null ? obj.AreaRootNodes.GetHashCode() : 0);
                hash = (13 * hash) ^ (obj.IterationRootNodes != null ? obj.IterationRootNodes.GetHashCode() : 0);

                return obj.WorkItemTypes.Aggregate(hash, (current, wit) => (13 * current) ^ wit.GetHashCode());
            }


        }

        private class Nested
        {
            internal static readonly ProjectComparer Instance = new ProjectComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}
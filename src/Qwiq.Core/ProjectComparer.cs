using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    public class ProjectComparer : GenericComparer<IProject>
    {
        public static IEqualityComparer<IProject> Instance => Nested.Instance;

        public override bool Equals(IProject x, IProject y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)
                   && x.WorkItemTypes.All(p => y.WorkItemTypes.Contains(p, WorkItemTypeComparer.Instance));
        }

        public override int GetHashCode(IProject obj)
        {
            return obj.Guid.GetHashCode();
        }

        private class Nested
        {
            internal static readonly IEqualityComparer<IProject> Instance = new ProjectComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}
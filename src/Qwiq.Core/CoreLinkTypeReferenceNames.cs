using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public static class CoreLinkTypeReferenceNames
    {
        public const string Dependency = "System.LinkTypes.Dependency";

        public const string Duplicate = "System.LinkTypes.Duplicate";

        public const string Hierarchy = "System.LinkTypes.Hierarchy";

        public const string Related = "System.LinkTypes.Related";

        /// <summary>Returns the set of all core link types.</summary>
        public static readonly IEnumerable<string> All = new[] { Related, Hierarchy, Dependency, Duplicate };
    }
}
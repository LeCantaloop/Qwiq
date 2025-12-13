using System;
using JetBrains.Annotations;

namespace Qwiq
{
    internal class QueryFolderComparer : GenericComparer<IQueryFolder>
    {
        internal new static readonly QueryFolderComparer Default = Nested.Instance;

        private QueryFolderComparer()
        {
        }

        public override bool Equals(IQueryFolder x, IQueryFolder y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            // Paths are not used for comparison because they differ in REST and SOAP due to inclusion or project
            var res = x.Id == y.Id
                      && string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)
                      && Comparer.QueryFolderCollection.Equals(x.SubFolders, y.SubFolders)
                      && Comparer.QueryDefinitionCollection.Equals(x.SavedQueries, y.SavedQueries);

            return res;
        }

        public override int GetHashCode([CanBeNull] IQueryFolder obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            unchecked
            {
                var hash = 27;

                hash = (hash * 13) ^ obj.Id.GetHashCode();
                hash = (hash * 13) ^ (obj.Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name) : 0);
                hash = (hash * 13) ^ obj.SubFolders.GetHashCode();
                hash = (hash * 13) ^ obj.SavedQueries.GetHashCode();

                return hash;
            }
        }

        private class Nested
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly QueryFolderComparer Instance = new QueryFolderComparer();

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
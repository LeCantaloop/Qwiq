using System;
using JetBrains.Annotations;

namespace Qwiq
{
    internal class QueryDefinitionComparer : GenericComparer<IQueryDefinition>
    {
        internal new static readonly QueryDefinitionComparer Default = Nested.Instance;

        private QueryDefinitionComparer()
        {
        }

        public override bool Equals(IQueryDefinition x, IQueryDefinition y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            var res = x.Id == y.Id
                    && string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.Path, y.Path, StringComparison.OrdinalIgnoreCase);

            return res;
        }

        public override int GetHashCode([CanBeNull] IQueryDefinition obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            unchecked
            {
                var hash = 27;

                hash = (hash * 13) ^ obj.Id.GetHashCode();
                hash = (hash * 13) ^ (obj.Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name) : 0);
                hash = (hash * 13) ^ (obj.Path != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Path) : 0);

                return hash;
            }
        }

        private class Nested
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly QueryDefinitionComparer Instance = new QueryDefinitionComparer();

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
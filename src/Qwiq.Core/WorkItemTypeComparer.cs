using System;


namespace Qwiq
{
    internal class WorkItemTypeComparer : GenericComparer<IWorkItemType>
    {
        internal new static WorkItemTypeComparer Default => Nested.Instance;

        private WorkItemTypeComparer()
        {
        }

        public override bool Equals(IWorkItemType x, IWorkItemType y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(x.Description, y.Description, StringComparison.OrdinalIgnoreCase)
                   && FieldDefinitionCollectionComparer.Default.Equals(x.FieldDefinitions, y.FieldDefinitions);
        }

        public override int GetHashCode(IWorkItemType obj)
        {
            if (ReferenceEquals(obj, null)) return 0;


            unchecked
            {
                var hash = 27;
                hash = (13 * hash) ^ (obj.Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name) : 0);
                hash = (13 * hash) ^ (obj.Description != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Description) : 0);
                hash = (13 * hash) ^ (obj.FieldDefinitions != null ? Comparer.FieldDefinitionCollection.GetHashCode(obj.FieldDefinitions) : 0);

                return hash;
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Nested
        {
            // ReSharper disable once MemberHidesStaticFromOuterClass
            internal static readonly WorkItemTypeComparer Instance = new WorkItemTypeComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}
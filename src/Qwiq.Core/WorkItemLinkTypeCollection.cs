using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Qwiq
{
    public class WorkItemLinkTypeCollection : ReadOnlyList<IWorkItemLinkType>, IWorkItemLinkTypeCollection
    {
        private readonly Lazy<IWorkItemLinkTypeEndCollection> _ltCol;

        public WorkItemLinkTypeCollection(IEnumerable<IWorkItemLinkType> linkTypes)
            : base(linkTypes, type => type.ReferenceName)
        {
            _ltCol = new Lazy<IWorkItemLinkTypeEndCollection>(() => new WorkItemLinkTypeEndCollection(this));
        }

        /// <summary>
        ///     Retrieves a collection of all the link type ends across all link types.
        ///     This is provided for convienience and faster lookup of link type ends
        ///     by Id, Name, and ImmutableName.
        /// </summary>
        public IWorkItemLinkTypeEndCollection LinkTypeEnds => _ltCol.Value;
        [DebuggerStepThrough]
        public bool Equals(IWorkItemLinkTypeCollection other)
        {
            return Equals((object)other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(obj, null)) return false;
            var ltc = obj as IEnumerable<IWorkItemLinkType>;
            if (ltc == null) return false;

            return this.All(p => ltc.Contains(p, WorkItemLinkTypeComparer.Instance));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // REVIEW: Order must be the same to generate equivilent hash codes
                return this.Aggregate(27, (current, l) => (13 * current) ^ l.GetHashCode());
            }
        }
    }
}
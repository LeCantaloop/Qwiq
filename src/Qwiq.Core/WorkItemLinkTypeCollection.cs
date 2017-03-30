using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    public class WorkItemLinkTypeCollection : IReadOnlyCollection<IWorkItemLinkType>
    {
        private readonly Dictionary<string, IWorkItemLinkType> _mapByName;

        public WorkItemLinkTypeCollection(IEnumerable<IWorkItemLinkType> linkTypes)
        {
            if (linkTypes == null) throw new ArgumentNullException(nameof(linkTypes));
            _mapByName = new Dictionary<string, IWorkItemLinkType>(StringComparer.OrdinalIgnoreCase);
            foreach (var linkType in linkTypes) _mapByName[linkType.ReferenceName] = linkType;
            LinkTypeEnds = new WorkItemLinkTypeEndCollection(_mapByName.Values);
        }

        public int Count => _mapByName.Count;

        /// <summary>
        ///     Retrieves a collection of all the link type ends across all link types.
        ///     This is provided for convienience and faster lookup of link type ends
        ///     by Id, Name, and ImmutableName.
        /// </summary>
        public WorkItemLinkTypeEndCollection LinkTypeEnds { get; }

        public IWorkItemLinkType this[string linkTypeReferenceName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(linkTypeReferenceName))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(linkTypeReferenceName));
                if (_mapByName.TryGetValue(linkTypeReferenceName, out IWorkItemLinkType end)) return end;

                throw new Exception($"Work item link type {linkTypeReferenceName} does not exist.");
            }
        }

        public IEnumerator<IWorkItemLinkType> GetEnumerator()
        {
            return _mapByName.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _mapByName.Values.GetEnumerator();
        }

        public bool Contains(string linkTypeReferenceName)
        {
            if (string.IsNullOrWhiteSpace(linkTypeReferenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(linkTypeReferenceName));
            return _mapByName.ContainsKey(linkTypeReferenceName);
        }

        public bool TryGetByName(string linkTypeReferenceName, out IWorkItemLinkType linkType)
        {
            if (string.IsNullOrWhiteSpace(linkTypeReferenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(linkTypeReferenceName));
            return _mapByName.TryGetValue(linkTypeReferenceName, out linkType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Aggregate(27, (current, l) => (13 * current) ^ l.GetHashCode());
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(obj, null)) return false;
            var ltc = obj as IEnumerable<IWorkItemLinkType>;
            if (ltc == null) return false;

            return this.All(p => ltc.Contains(p, WorkItemLinkTypeComparer.Instance));
        }
    }
}
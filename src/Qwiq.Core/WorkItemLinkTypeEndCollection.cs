using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    

    public class WorkItemLinkTypeEndCollection : IReadOnlyCollection<IWorkItemLinkTypeEnd>
    {
        private readonly Dictionary<string, IWorkItemLinkTypeEnd> _mapByName;

        public WorkItemLinkTypeEndCollection(IEnumerable<IWorkItemLinkType> linkTypes)
        {
            if (linkTypes == null) throw new ArgumentNullException(nameof(linkTypes));

            _mapByName = new Dictionary<string, IWorkItemLinkTypeEnd>(StringComparer.OrdinalIgnoreCase);
            foreach (var linkType in linkTypes)
            {
                _mapByName[linkType.ForwardEnd.ImmutableName] = linkType.ForwardEnd;
                _mapByName[linkType.ForwardEnd.Name] = linkType.ForwardEnd;
                if (linkType.IsDirectional)
                {
                    _mapByName[linkType.ReverseEnd.ImmutableName] = linkType.ReverseEnd;
                    _mapByName[linkType.ReverseEnd.Name] = linkType.ReverseEnd;
                }
            }
        }

        public int Count => _mapByName.Count;

        public IWorkItemLinkTypeEnd this[string linkTypeEndName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(linkTypeEndName))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(linkTypeEndName));
                IWorkItemLinkTypeEnd end;
                if (_mapByName.TryGetValue(linkTypeEndName, out end)) return end;

                throw new Exception($"Work item link type {linkTypeEndName} does not exist.");
            }
        }

        public IEnumerator<IWorkItemLinkTypeEnd> GetEnumerator()
        {
            return _mapByName.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _mapByName.Values.GetEnumerator();
        }

        public bool Contains(string linkTypeEndName)
        {
            if (string.IsNullOrWhiteSpace(linkTypeEndName)) return false;

            return _mapByName.ContainsKey(linkTypeEndName);
        }

        public bool TryGetByName(string linkTypeEndName, out IWorkItemLinkTypeEnd linkTypeEnd)
        {
            if (string.IsNullOrWhiteSpace(linkTypeEndName))
            {
                linkTypeEnd = null;
                return false;
            }

            return _mapByName.TryGetValue(linkTypeEndName, out linkTypeEnd);
        }
    }
}
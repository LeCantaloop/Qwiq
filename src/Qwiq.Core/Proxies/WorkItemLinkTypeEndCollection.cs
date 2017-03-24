using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Proxies
{
    public class WorkItemLinkTypeCollection : IEnumerable<IWorkItemLinkType>
    {
        private readonly Dictionary<string, IWorkItemLinkType> _mapByName;
        private WorkItemLinkTypeEndCollection _endsCollection;

        public int Count => _mapByName.Count;

        public bool Contains(string linkTypeReferenceName)
        {
            return _mapByName.ContainsKey(linkTypeReferenceName);
        }

        /// <summary>
        /// Retrieves a collection of all the link type ends across all link types.
        /// This is provided for convienience and faster lookup of link type ends
        /// by Id, Name, and ImmutableName.
        /// </summary>
        public WorkItemLinkTypeEndCollection LinkTypeEnds => this._endsCollection;

        public IWorkItemLinkType this[string linkTypeReferenceName]
        {
            get
            {
                IWorkItemLinkType end;
                if (_mapByName.TryGetValue(linkTypeReferenceName, out end))
                {
                    return end;
                }

                throw new Exception($"Work item link type {linkTypeReferenceName} does not exist.");
            }
        }

        public bool TryGetByName(string linkTypeReferenceName, out IWorkItemLinkType linkType)
        {
            return _mapByName.TryGetValue(linkTypeReferenceName, out linkType);
        }

        public IEnumerator<IWorkItemLinkType> GetEnumerator()
        {
            return _mapByName.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _mapByName.Values.GetEnumerator();
        }

        public WorkItemLinkTypeCollection(IEnumerable<IWorkItemLinkType> linkTypes)
        {
            if (linkTypes == null) throw new ArgumentNullException(nameof(linkTypes));
            _mapByName = new Dictionary<string, IWorkItemLinkType>(StringComparer.OrdinalIgnoreCase);
            foreach (var linkType in linkTypes)
            {
                _mapByName[linkType.ReferenceName] = linkType;
            }
            _endsCollection = new WorkItemLinkTypeEndCollection(_mapByName.Values);
        }
    }

    public class WorkItemLinkTypeEndCollection : IEnumerable<IWorkItemLinkTypeEnd>
    {
        private readonly Dictionary<string, IWorkItemLinkTypeEnd> _mapByName;



        public int Count => _mapByName.Count;

        public bool Contains(string linkTypeName)
        {
            return _mapByName.ContainsKey(linkTypeName);
        }

        public IWorkItemLinkTypeEnd this[string linkTypeEndName]
        {
            get
            {
                IWorkItemLinkTypeEnd end;
                if (_mapByName.TryGetValue(linkTypeEndName, out end))
                {
                    return end;
                }

                throw new Exception($"Work item link type {linkTypeEndName} does not exist.");
            }
        }

        public bool TryGetByName(string linkTypeEndName, out IWorkItemLinkTypeEnd linkTypeEnd)
        {
            return _mapByName.TryGetValue(linkTypeEndName, out linkTypeEnd);
        }

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

        public IEnumerator<IWorkItemLinkTypeEnd> GetEnumerator()
        {
            return _mapByName.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _mapByName.Values.GetEnumerator();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public class LinkCollectionProxy : ICollection<ILink>
    {
        private readonly Tfs.WorkItem _item;
        private readonly LinkMapper _linkMapper;

        internal LinkCollectionProxy(Tfs.WorkItem item)
        {
            _item = item;
            _linkMapper = new LinkMapper();
        }

        public IEnumerator<ILink> GetEnumerator()
        {
            return _item.Links.Cast<Tfs.Link>().Select(link => new LinkProxy(link)).Cast<ILink>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ILink item)
        {
            var concreteLink = _linkMapper.Map(item, _item);
            _item.Links.Add(concreteLink);
        }

        public void Clear()
        {
            _item.Links.Clear();
        }

        public bool Contains(ILink item)
        {
            var concreteLink = _linkMapper.Map(item, _item);
            return _item.Links.Contains(concreteLink);
        }

        public void CopyTo(ILink[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(ILink item)
        {
            var concreteLink = _linkMapper.Map(item, _item);

            var wasFound = Contains(item);
            if (wasFound)
            {
                _item.Links.Remove(concreteLink);
            }
            return wasFound;
        }

        public int Count
        {
            get { return _item.Links.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IList) _item.Links).IsReadOnly; }
        }
    }
}

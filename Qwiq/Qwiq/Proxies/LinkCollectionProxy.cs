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
        internal LinkCollectionProxy(Tfs.WorkItem item)
        {
            _item = item;
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
            _item.Links.Add((item as LinkProxy).Link);
        }

        public void Clear()
        {
            _item.Links.Clear();
        }

        public bool Contains(ILink item)
        {
            return _item.Links.Contains((item as LinkProxy).Link);
        }

        public void CopyTo(ILink[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(ILink item)
        {
            var realLink = _item.Links.Cast<Tfs.Link>().Single(link => link == (item as LinkProxy).Link);
            _item.Links.Remove(realLink);
            return true;
        }

        public int Count
        {
            get { return _item.Links.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IList)_item.Links).IsReadOnly; }
        }
    }
}

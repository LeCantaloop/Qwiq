using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    public class LinkCollectionProxy : ICollection<ILink>
    {
        private readonly Tfs.WorkItem _item;
        private readonly LinkHelper _linkHelper;
        private readonly LinkMapper _linkMapper;

        internal LinkCollectionProxy(Tfs.WorkItem item)
        {
            _item = item;
            _linkHelper = new LinkHelper();
            _linkMapper = new LinkMapper();
        }

        public IEnumerator<ILink> GetEnumerator()
        {
            return _item.Links.Cast<Tfs.Link>().Select(link => _linkMapper.Map(link)).GetEnumerator();
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
            return _linkHelper.FindEquivalentLink(_item, item) != null;
        }

        public void CopyTo(ILink[] array, int arrayIndex)
        {
            var j = 0;
            for (var i = arrayIndex; i < _item.Links.Count && j < array.Length; i++)
            {
                var link = _item.Links[i];
                array[j] = _linkMapper.Map(link);
                j++;
            }
        }

        public bool Remove(ILink item)
        {
            var link = _linkHelper.FindEquivalentLink(_item, item);

            var wasFound = _item.Links.Contains(link);
            if (wasFound)
            {
                _item.Links.Remove(link);
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


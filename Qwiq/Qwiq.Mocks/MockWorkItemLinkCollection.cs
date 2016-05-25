using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Mocks
{
    public class MockLinkCollection : ICollection<ILink>
    {
        private readonly ICollection<ILink> _links;

        public MockLinkCollection()
            : this(new List<ILink>())
        {
        }

        public MockLinkCollection(ICollection<ILink> links)
        {
            _links = links;
        }

        public IEnumerator<ILink> GetEnumerator()
        {
            return _links.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ILink item)
        {
            _links.Add(item);
        }

        public void Clear()
        {
            _links.Clear();
        }

        public bool Contains(ILink item)
        {
            return _links.Contains(item);
        }

        public void CopyTo(ILink[] array, int arrayIndex)
        {
            _links.CopyTo(array, arrayIndex);
        }

        public bool Remove(ILink item)
        {
            return _links.Remove(item);
        }

        public int Count => _links.Count;

        public bool IsReadOnly => _links.IsReadOnly;

        public ILink[] Links => _links.ToArray();
    }
}
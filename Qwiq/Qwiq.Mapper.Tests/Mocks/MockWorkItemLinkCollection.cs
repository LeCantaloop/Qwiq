using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Mapper.Tests.Mocks
{
    public class MockLinkCollection : ICollection<ILink>
    {
        public IEnumerator<ILink> GetEnumerator()
        {
            return Links.Cast<ILink>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ILink item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(ILink item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(ILink[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(ILink item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; set; }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public ILink[] Links { get; set; }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Mapper.Tests.Mocks
{
    public class MockLinkCollection : ICollection<IWorkItemLink>
    {
        public IEnumerator<IWorkItemLink> GetEnumerator()
        {
            return Links.Cast<IWorkItemLink>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IWorkItemLink item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IWorkItemLink item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IWorkItemLink[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IWorkItemLink item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; set; }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public IWorkItemLink[] Links { get; set; }
    }
}
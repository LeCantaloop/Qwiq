using System;

namespace Microsoft.IE.Qwiq.Mapper.Tests.Mocks
{
    public class MockWorkItemLinkTypeEnd : IWorkItemLinkTypeEnd
    {
        public int Id
        {
            get { throw new NotImplementedException(); }
        }

        public string ImmutableName { get; set; }

        public bool IsForwardLink
        {
            get { throw new NotImplementedException(); }
        }

        public IWorkItemLinkType LinkType
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public IWorkItemLinkTypeEnd OppositeEnd
        {
            get { throw new NotImplementedException(); }
        }
    }
}

using System;

namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemLinkTypeEnd : IWorkItemLinkTypeEnd
    {
        public MockWorkItemLinkTypeEnd(string parentName, string direction, string name)
            : this(parentName + "-" + direction, name)
        {
        }

        public MockWorkItemLinkTypeEnd(string immutableName, string name)
        {
            ImmutableName = immutableName;
            Name = name;
        }

        public int Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ImmutableName { get; }

        public bool IsForwardLink => Name.Equals("Child", StringComparison.OrdinalIgnoreCase)
                                     || Name.Equals("Forward", StringComparison.OrdinalIgnoreCase);

        public IWorkItemLinkType LinkType { get; set; }

        public string Name { get; }

        public IWorkItemLinkTypeEnd OppositeEnd
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}

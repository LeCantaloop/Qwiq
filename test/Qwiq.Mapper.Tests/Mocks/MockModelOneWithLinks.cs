using System.Collections.Generic;
using System.Linq;
using Microsoft.Qwiq.Mapper.Attributes;

namespace Microsoft.Qwiq.Mapper.Tests.Mocks
{
    [WorkItemType("Baz")]
    public class MockModelOneWithLinks : MockModelOne
    {
        public const string ReverseLinkName = "NS.SampleLink-Reverse";
        public const string ForwardLinkName = "NS.SampleLink-Forward";

        private IEnumerable<MockModelOne> _givers;
        private IEnumerable<MockModelOne> _takers;

        [WorkItemLink(typeof(MockModelOne), ReverseLinkName)]
        public IEnumerable<MockModelOne> Givers
        {
            get { return (_givers ?? Enumerable.Empty<MockModelOne>()); }
            internal set { _givers = value; }
        }

        [WorkItemLink(typeof(MockModelOne), ForwardLinkName)]
        public IEnumerable<MockModelOne> Takers
        {
            get { return (_takers ?? Enumerable.Empty<MockModelOne>()); }
            internal set { _takers = value; }
        }
    }
}


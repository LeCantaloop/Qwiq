using System.Collections.Generic;
using System.Linq;
using Microsoft.Qwiq.Mapper.Attributes;

namespace Microsoft.Qwiq.Mapper.Tests.Mocks
{
    [WorkItemType("MockWorkItem")]
    public class MockModelWithLinks : MockModel
    {
        public const string ReverseLinkName = "NS.SampleLink-Reverse";
        public const string ForwardLinkName = "NS.SampleLink-Forward";

        private IEnumerable<MockModel> _givers;
        private IEnumerable<MockModel> _takers;

        [WorkItemLink(typeof(MockModel), ReverseLinkName)]
        public IEnumerable<MockModel> Givers
        {
            get { return _givers ?? Enumerable.Empty<MockModel>(); }
            internal set { _givers = value; }
        }

        [WorkItemLink(typeof(MockModel), ForwardLinkName)]
        public IEnumerable<MockModel> Takers
        {
            get { return _takers ?? Enumerable.Empty<MockModel>(); }
            internal set { _takers = value; }
        }
    }
}


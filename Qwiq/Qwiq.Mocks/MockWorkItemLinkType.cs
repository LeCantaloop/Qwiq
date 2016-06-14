using System;

using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq.Mocks
{
    public class MockWorkItemLinkType : IWorkItemLinkType
    {
        private readonly string _forwardName;

        private readonly string _reverseName;

        public MockWorkItemLinkType(string referenceName)
        {
            if (referenceName == CoreLinkTypeReferenceNames.Hierarchy)
            {
                _forwardName = "Child";
                _reverseName = "Parent";
            }
            else if (referenceName == CoreLinkTypeReferenceNames.Related)
            {
                _forwardName = _reverseName = "Related";
            }
            else if (referenceName == CoreLinkTypeReferenceNames.Dependency)
            {
                _forwardName = "Successor";
                _reverseName = "Predecessor";
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    nameof(referenceName),
                    referenceName,
                    "Reference name not supported in mock object.");
            }
            ReferenceName = referenceName;
        }

        public IWorkItemLinkTypeEnd ForwardEnd => new MockWorkItemLinkTypeEnd(ReferenceName, "Forward", _forwardName) { LinkType = this };

        public bool IsActive => true;

        public string ReferenceName { get; }

        public IWorkItemLinkTypeEnd ReverseEnd => new MockWorkItemLinkTypeEnd(
            ReferenceName,
            ReferenceName.Equals("System.LinkTypes.Related") ? "Forward" : "Reverse",
            _reverseName) { LinkType = this };

       
    }
}
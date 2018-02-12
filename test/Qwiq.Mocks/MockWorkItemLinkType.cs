using System;

namespace Qwiq.Mocks
{
    public class MockWorkItemLinkType : WorkItemLinkType
    {
        public MockWorkItemLinkType(string referenceName)
            : base(referenceName)
        {
            string reverseName;
            int reverseId = 0;
            string forwardName;
            int forwardId = 0;

            if (CoreLinkTypeReferenceNames.Hierarchy.Equals(referenceName, StringComparison.OrdinalIgnoreCase))
            {
                // The forward should be Child, but the ID used in CoreLinkTypes is -2, should be 2
                forwardName = "Child";
                forwardId = CoreLinkTypes.Child;
                reverseName = "Parent";
                reverseId = CoreLinkTypes.Parent;
                IsDirectional = true;
            }
            else if (CoreLinkTypeReferenceNames.Related.Equals(referenceName, StringComparison.OrdinalIgnoreCase))
            {
                forwardName = reverseName = "Related";
                forwardId = reverseId = -CoreLinkTypes.Related;
                IsDirectional = false;
            }
            else if (CoreLinkTypeReferenceNames.Dependency.Equals(referenceName, StringComparison.OrdinalIgnoreCase))
            {
                forwardName = "Successor";
                forwardId = CoreLinkTypes.Successor;
                reverseName = "Predecessor";
                reverseId = CoreLinkTypes.Predecessor;
                IsDirectional = true;
            }
            else if (CoreLinkTypeReferenceNames.Duplicate.Equals(referenceName, StringComparison.OrdinalIgnoreCase))
            {
                forwardName = "Duplicate";
                reverseName = "Duplicate Of";
                IsDirectional = true;
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    nameof(referenceName),
                    referenceName,
                    "Reference name not supported in mock object.");
            }

            SetForwardEnd(new MockWorkItemLinkTypeEnd(this, forwardName, true, -forwardId));
            SetReverseEnd(new MockWorkItemLinkTypeEnd(this, reverseName, false, -reverseId));
        }

        public MockWorkItemLinkType(
            string referenceName,
            bool isDirectional,
            string forwardEndName,
            string reverseEndName,
            int id = 0)
            : base(referenceName)
        {
            if (string.IsNullOrEmpty(forwardEndName)) throw new ArgumentException("Value cannot be null or empty.", nameof(forwardEndName));
            if (string.IsNullOrEmpty(reverseEndName)) throw new ArgumentException("Value cannot be null or empty.", nameof(reverseEndName));
            IsDirectional = isDirectional;
            SetForwardEnd(new MockWorkItemLinkTypeEnd(this, forwardEndName, true, id));
            SetReverseEnd(new MockWorkItemLinkTypeEnd(this, reverseEndName, false, -id));
        }


    }
}
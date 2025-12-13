using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Links
{
    /// <summary>
    /// Tests for RelatedLink construction and behavior.
    /// </summary>
    [TestClass]
    public class Given_valid_RelatedLink_parameters : ContextSpecification
    {
        private MockRelatedLink _link = null!;
        private MockWorkItemLinkTypeEnd _linkTypeEnd = null!;
        private int _targetId;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            _linkTypeEnd = (MockWorkItemLinkTypeEnd)linkType.ForwardEnd;
            _targetId = 456;
        }

        public override void When()
        {
            _link = new MockRelatedLink(_linkTypeEnd, _targetId);
        }

        [TestMethod]
        public void Then_RelatedWorkItemId_is_set()
        {
            _link.RelatedWorkItemId.ShouldBe(_targetId);
        }

        [TestMethod]
        public void Then_LinkTypeEnd_is_set()
        {
            _link.LinkTypeEnd.ShouldBe(_linkTypeEnd);
        }

        [TestMethod]
        public void Then_BaseType_is_RelatedLink()
        {
            _link.BaseType.ShouldBe(BaseLinkType.RelatedLink);
        }
    }

    [TestClass]
    public class Given_two_RelatedLinks_with_same_values : ContextSpecification
    {
        private MockRelatedLink _link1 = null!;
        private MockRelatedLink _link2 = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkTypeEnd = linkType.ForwardEnd;
            _link1 = new MockRelatedLink(linkTypeEnd, 456);
            _link2 = new MockRelatedLink(linkTypeEnd, 456);
        }

        [TestMethod]
        public void Then_Equals_returns_true()
        {
            _link1.Equals(_link2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_is_same()
        {
            _link1.GetHashCode().ShouldBe(_link2.GetHashCode());
        }
    }

    [TestClass]
    public class Given_two_RelatedLinks_with_different_target_id : ContextSpecification
    {
        private MockRelatedLink _link1 = null!;
        private MockRelatedLink _link2 = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkTypeEnd = linkType.ForwardEnd;
            _link1 = new MockRelatedLink(linkTypeEnd, 100);
            _link2 = new MockRelatedLink(linkTypeEnd, 200);
        }

        [TestMethod]
        public void Then_Equals_returns_false()
        {
            _link1.Equals(_link2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_two_RelatedLinks_with_different_link_type_end : ContextSpecification
    {
        private MockRelatedLink _link1 = null!;
        private MockRelatedLink _link2 = null!;

        public override void Given()
        {
            var linkType1 = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkTypeEnd1 = linkType1.ForwardEnd;

            var linkType2 = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Hierarchy);
            var linkTypeEnd2 = linkType2.ForwardEnd;

            _link1 = new MockRelatedLink(linkTypeEnd1, 456);
            _link2 = new MockRelatedLink(linkTypeEnd2, 456);
        }

        [TestMethod]
        public void Then_Equals_returns_false()
        {
            _link1.Equals(_link2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_RelatedLink_Equals_with_null : ContextSpecification
    {
        private MockRelatedLink _link = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkTypeEnd = linkType.ForwardEnd;
            _link = new MockRelatedLink(linkTypeEnd, 456);
        }

        [TestMethod]
        public void Then_Equals_IRelatedLink_null_returns_false()
        {
            _link.Equals((IRelatedLink?)null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_object_null_returns_false()
        {
            _link.Equals((object?)null).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_RelatedLink_Equals_with_same_reference : ContextSpecification
    {
        private MockRelatedLink _link = null!;
        private bool _result;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkTypeEnd = linkType.ForwardEnd;
            _link = new MockRelatedLink(linkTypeEnd, 456);
        }

        public override void When()
        {
            _result = _link.Equals(_link);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_RelatedLink_with_null_link_type_end : ContextSpecification
    {
        private MockRelatedLink _link = null!;

        public override void Given()
        {
            _link = new MockRelatedLink(null, 456);
        }

        [TestMethod]
        public void Then_LinkTypeEnd_is_null()
        {
            _link.LinkTypeEnd.ShouldBeNull();
        }

        [TestMethod]
        public void Then_GetHashCode_returns_value()
        {
            // Should not throw, just verify it returns a value
            var hash = _link.GetHashCode();
            hash.ShouldBeOfType<int>();
        }
    }
}

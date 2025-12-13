using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Extensions
{
    /// <summary>
    /// Tests for IWorkItemLinkTypeEndExtensions.LinkTypeId()
    /// MockWorkItemLinkTypeEnd implements IIdentifiable&lt;int&gt; so returns the Id property directly.
    /// </summary>
    [TestClass]
    public class Given_LinkTypeEnd_with_explicit_Id : ContextSpecification
    {
        private IWorkItemLinkTypeEnd _linkTypeEnd = null!;
        private int _result;
        private const int ExpectedId = 42;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType("System.LinkTypes.Related", true, "Related", "Related", ExpectedId);
            _linkTypeEnd = linkType.ForwardEnd;
        }

        public override void When()
        {
            _result = _linkTypeEnd.LinkTypeId();
        }

        [TestMethod]
        public void Then_returns_the_Id()
        {
            _result.ShouldBe(ExpectedId);
        }
    }

    [TestClass]
    public class Given_LinkTypeEnd_reverse_with_explicit_Id : ContextSpecification
    {
        private IWorkItemLinkTypeEnd _linkTypeEnd = null!;
        private int _result;
        private const int ExpectedId = -42;

        public override void Given()
        {
            // Constructor sets reverse end to -id, so id=42 gives reverse=-42
            var linkType = new MockWorkItemLinkType("System.LinkTypes.Related", true, "Related", "Related", 42);
            _linkTypeEnd = linkType.ReverseEnd;
        }

        public override void When()
        {
            _result = _linkTypeEnd.LinkTypeId();
        }

        [TestMethod]
        public void Then_returns_the_negative_Id()
        {
            _result.ShouldBe(ExpectedId);
        }
    }

    [TestClass]
    public class Given_null_LinkTypeEnd : ContextSpecification
    {
        private IWorkItemLinkTypeEnd? _linkTypeEnd;
        private int _result;

        public override void Given()
        {
            _linkTypeEnd = null;
        }

        public override void When()
        {
            _result = _linkTypeEnd!.LinkTypeId();
        }

        [TestMethod]
        public void Then_returns_zero()
        {
            _result.ShouldBe(0);
        }
    }

    /// <summary>
    /// Tests for IWorkItemLinkTypeExtensions.ForwardEndLinkTypeId() and ReverseEndLinkTypeId()
    /// </summary>
    [TestClass]
    public class Given_LinkType_with_explicit_end_ids : ContextSpecification
    {
        private IWorkItemLinkType _linkType = null!;
        private int _forwardId;
        private int _reverseId;
        private const int ForwardEndId = 100;
        private const int ReverseEndId = -100;

        public override void Given()
        {
            _linkType = new MockWorkItemLinkType("System.LinkTypes.Hierarchy", false, "Child", "Parent", ForwardEndId);
        }

        public override void When()
        {
            _forwardId = _linkType.ForwardEndLinkTypeId();
            _reverseId = _linkType.ReverseEndLinkTypeId();
        }

        [TestMethod]
        public void Then_ForwardEndLinkTypeId_returns_expected()
        {
            _forwardId.ShouldBe(ForwardEndId);
        }

        [TestMethod]
        public void Then_ReverseEndLinkTypeId_returns_expected()
        {
            _reverseId.ShouldBe(ReverseEndId);
        }
    }

    [TestClass]
    public class Given_null_LinkType_for_ForwardEndLinkTypeId : ContextSpecification
    {
        private IWorkItemLinkType? _linkType;
        private int _result;

        public override void Given()
        {
            _linkType = null;
        }

        public override void When()
        {
            _result = _linkType!.ForwardEndLinkTypeId();
        }

        [TestMethod]
        public void Then_returns_zero()
        {
            _result.ShouldBe(0);
        }
    }

    [TestClass]
    public class Given_null_LinkType_for_ReverseEndLinkTypeId : ContextSpecification
    {
        private IWorkItemLinkType? _linkType;
        private int _result;

        public override void Given()
        {
            _linkType = null;
        }

        public override void When()
        {
            _result = _linkType!.ReverseEndLinkTypeId();
        }

        [TestMethod]
        public void Then_returns_zero()
        {
            _result.ShouldBe(0);
        }
    }

    /// <summary>
    /// Tests for IWorkItemLinkInfoExtensions.LinkTypeId()
    /// </summary>
    [TestClass]
    public class Given_WorkItemLinkInfo_with_LinkTypeEnd : ContextSpecification
    {
        private IWorkItemLinkInfo _linkInfo = null!;
        private int _result;
        private const int ExpectedId = 77;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType("System.LinkTypes.Related", true, "Related", "Related", ExpectedId);
            _linkInfo = new WorkItemLinkInfo(100, 200, linkType.ForwardEnd);
        }

        public override void When()
        {
            _result = _linkInfo.LinkTypeId();
        }

        [TestMethod]
        public void Then_returns_link_type_id()
        {
            _result.ShouldBe(ExpectedId);
        }
    }

    [TestClass]
    public class Given_null_WorkItemLinkInfo : ContextSpecification
    {
        private IWorkItemLinkInfo? _linkInfo;
        private int _result;

        public override void Given()
        {
            _linkInfo = null;
        }

        public override void When()
        {
            _result = _linkInfo!.LinkTypeId();
        }

        [TestMethod]
        public void Then_returns_zero()
        {
            _result.ShouldBe(0);
        }
    }
}

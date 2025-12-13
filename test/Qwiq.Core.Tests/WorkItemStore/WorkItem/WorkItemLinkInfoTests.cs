using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.WorkItemStore.WorkItem
{
    /// <summary>
    /// Tests for WorkItemLinkInfo construction and behavior.
    /// </summary>
    [TestClass]
    public class Given_valid_WorkItemLinkInfo_parameters : ContextSpecification
    {
        private WorkItemLinkInfo _linkInfo = null!;
        private IWorkItemLinkTypeEnd _linkTypeEnd = null!;
        private int _sourceId;
        private int _targetId;

        public override void Given()
        {
            _sourceId = 100;
            _targetId = 200;
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            _linkTypeEnd = linkType.ForwardEnd;
        }

        public override void When()
        {
            _linkInfo = new WorkItemLinkInfo(_sourceId, _targetId, _linkTypeEnd);
        }

        [TestMethod]
        public void Then_SourceId_is_set()
        {
            _linkInfo.SourceId.ShouldBe(_sourceId);
        }

        [TestMethod]
        public void Then_TargetId_is_set()
        {
            _linkInfo.TargetId.ShouldBe(_targetId);
        }

        [TestMethod]
        public void Then_LinkType_is_set()
        {
            _linkInfo.LinkType.ShouldBe(_linkTypeEnd);
        }
    }

    [TestClass]
    public class Given_WorkItemLinkInfo_with_lazy_link_type : ContextSpecification
    {
        private WorkItemLinkInfo _linkInfo = null!;
        private IWorkItemLinkTypeEnd _linkTypeEnd = null!;
        private int _sourceId;
        private int _targetId;

        public override void Given()
        {
            _sourceId = 100;
            _targetId = 200;
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            _linkTypeEnd = linkType.ForwardEnd;
        }

        public override void When()
        {
            var lazyLinkTypeEnd = new Lazy<IWorkItemLinkTypeEnd?>(() => _linkTypeEnd);
            _linkInfo = new WorkItemLinkInfo(_sourceId, _targetId, lazyLinkTypeEnd);
        }

        [TestMethod]
        public void Then_SourceId_is_set()
        {
            _linkInfo.SourceId.ShouldBe(_sourceId);
        }

        [TestMethod]
        public void Then_TargetId_is_set()
        {
            _linkInfo.TargetId.ShouldBe(_targetId);
        }

        [TestMethod]
        public void Then_LinkType_is_resolved()
        {
            _linkInfo.LinkType.ShouldBe(_linkTypeEnd);
        }

        [TestMethod]
        public void Then_LinkType_is_cached()
        {
            // Access twice to verify caching
            var first = _linkInfo.LinkType;
            var second = _linkInfo.LinkType;
            first.ShouldBeSameAs(second);
        }
    }

    [TestClass]
    public class Given_WorkItemLinkInfo_with_null_link_type : ContextSpecification
    {
        private WorkItemLinkInfo _linkInfo = null!;

        public override void When()
        {
            _linkInfo = new WorkItemLinkInfo(100, 200, (IWorkItemLinkTypeEnd?)null);
        }

        [TestMethod]
        public void Then_LinkType_is_null()
        {
            _linkInfo.LinkType.ShouldBeNull();
        }
    }

    [TestClass]
    public class Given_WorkItemLinkInfo_with_null_lazy_throws : ContextSpecification
    {
        private Exception _exception = null!;

        public override void When()
        {
            try
            {
                _ = new WorkItemLinkInfo(100, 200, (Lazy<IWorkItemLinkTypeEnd?>?)null);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [TestMethod]
        public void Then_throws_ArgumentNullException()
        {
            _exception.ShouldBeOfType<ArgumentNullException>();
        }
    }

    [TestClass]
    public class Given_two_WorkItemLinkInfos_with_same_values : ContextSpecification
    {
        private WorkItemLinkInfo _linkInfo1 = null!;
        private WorkItemLinkInfo _linkInfo2 = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkTypeEnd = linkType.ForwardEnd;
            _linkInfo1 = new WorkItemLinkInfo(100, 200, linkTypeEnd);
            _linkInfo2 = new WorkItemLinkInfo(100, 200, linkTypeEnd);
        }

        [TestMethod]
        public void Then_Equals_returns_true()
        {
            _linkInfo1.Equals(_linkInfo2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_is_same()
        {
            _linkInfo1.GetHashCode().ShouldBe(_linkInfo2.GetHashCode());
        }
    }

    [TestClass]
    public class Given_two_WorkItemLinkInfos_with_different_sourceId : ContextSpecification
    {
        private WorkItemLinkInfo _linkInfo1 = null!;
        private WorkItemLinkInfo _linkInfo2 = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkTypeEnd = linkType.ForwardEnd;
            _linkInfo1 = new WorkItemLinkInfo(100, 200, linkTypeEnd);
            _linkInfo2 = new WorkItemLinkInfo(999, 200, linkTypeEnd);
        }

        [TestMethod]
        public void Then_Equals_returns_false()
        {
            _linkInfo1.Equals(_linkInfo2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_two_WorkItemLinkInfos_with_different_targetId : ContextSpecification
    {
        private WorkItemLinkInfo _linkInfo1 = null!;
        private WorkItemLinkInfo _linkInfo2 = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkTypeEnd = linkType.ForwardEnd;
            _linkInfo1 = new WorkItemLinkInfo(100, 200, linkTypeEnd);
            _linkInfo2 = new WorkItemLinkInfo(100, 999, linkTypeEnd);
        }

        [TestMethod]
        public void Then_Equals_returns_false()
        {
            _linkInfo1.Equals(_linkInfo2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_WorkItemLinkInfo_Equals_with_null : ContextSpecification
    {
        private WorkItemLinkInfo _linkInfo = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkTypeEnd = linkType.ForwardEnd;
            _linkInfo = new WorkItemLinkInfo(100, 200, linkTypeEnd);
        }

        [TestMethod]
        public void Then_Equals_IWorkItemLinkInfo_null_returns_false()
        {
            _linkInfo.Equals((IWorkItemLinkInfo?)null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_object_null_returns_false()
        {
            _linkInfo.Equals((object?)null).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_WorkItemLinkInfo_Equals_with_same_reference : ContextSpecification
    {
        private WorkItemLinkInfo _linkInfo = null!;
        private bool _result;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkTypeEnd = linkType.ForwardEnd;
            _linkInfo = new WorkItemLinkInfo(100, 200, linkTypeEnd);
        }

        public override void When()
        {
            _result = _linkInfo.Equals(_linkInfo);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_WorkItemLinkInfo_ToString : ContextSpecification
    {
        private WorkItemLinkInfo _linkInfo = null!;
        private string _result = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkTypeEnd = linkType.ForwardEnd;
            _linkInfo = new WorkItemLinkInfo(100, 200, linkTypeEnd);
        }

        public override void When()
        {
            _result = _linkInfo.ToString();
        }

        [TestMethod]
        public void Then_contains_SourceId()
        {
            _result.ShouldContain("S:100");
        }

        [TestMethod]
        public void Then_contains_TargetId()
        {
            _result.ShouldContain("T:200");
        }

        [TestMethod]
        public void Then_contains_Type()
        {
            _result.ShouldContain("Type:");
        }
    }

    [TestClass]
    public class Given_WorkItemLinkInfo_ToString_with_null_link_type : ContextSpecification
    {
        private WorkItemLinkInfo _linkInfo = null!;
        private string _result = null!;

        public override void Given()
        {
            _linkInfo = new WorkItemLinkInfo(100, 200, (IWorkItemLinkTypeEnd?)null);
        }

        public override void When()
        {
            _result = _linkInfo.ToString();
        }

        [TestMethod]
        public void Then_contains_UNKNOWN_type()
        {
            _result.ShouldContain("Type:UNKNOWN");
        }
    }

    /// <summary>
    /// Tests for WorkItemLinkInfoComparer.
    /// </summary>
    [TestClass]
    public class Given_WorkItemLinkInfoComparer_with_null : ContextSpecification
    {
        private WorkItemLinkInfo _linkInfo = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkTypeEnd = linkType.ForwardEnd;
            _linkInfo = new WorkItemLinkInfo(100, 200, linkTypeEnd);
        }

        [TestMethod]
        public void Then_null_null_returns_true()
        {
            WorkItemLinkInfoComparer.Default.Equals(null, null).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_linkInfo_null_returns_false()
        {
            WorkItemLinkInfoComparer.Default.Equals(_linkInfo, null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_null_linkInfo_returns_false()
        {
            WorkItemLinkInfoComparer.Default.Equals(null, _linkInfo).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_GetHashCode_null_returns_zero()
        {
            WorkItemLinkInfoComparer.Default.GetHashCode(null!).ShouldBe(0);
        }

        [TestMethod]
        public void Then_GetHashCode_returns_non_zero()
        {
            WorkItemLinkInfoComparer.Default.GetHashCode(_linkInfo).ShouldNotBe(0);
        }
    }

    [TestClass]
    public class Given_WorkItemLinkInfoComparer_with_same_reference : ContextSpecification
    {
        private bool _result;
        private WorkItemLinkInfo _linkInfo = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkTypeEnd = linkType.ForwardEnd;
            _linkInfo = new WorkItemLinkInfo(100, 200, linkTypeEnd);
        }

        public override void When()
        {
            _result = WorkItemLinkInfoComparer.Default.Equals(_linkInfo, _linkInfo);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }
}

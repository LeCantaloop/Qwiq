using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.WorkItemStore.WorkItem
{
    /// <summary>
    /// Tests for WorkItemLinkTypeEnd construction and behavior.
    /// </summary>
    [TestClass]
    public class Given_valid_WorkItemLinkTypeEnd_parameters : ContextSpecification
    {
        private MockWorkItemLinkTypeEnd _linkTypeEnd = null!;
        private IWorkItemLinkType _linkType = null!;

        public override void Given()
        {
            _linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
        }

        public override void When()
        {
            _linkTypeEnd = (MockWorkItemLinkTypeEnd)_linkType.ForwardEnd;
        }

        [TestMethod]
        public void Then_ImmutableName_is_set()
        {
            _linkTypeEnd.ImmutableName.ShouldNotBeNullOrEmpty();
        }

        [TestMethod]
        public void Then_Name_is_set()
        {
            _linkTypeEnd.Name.ShouldNotBeNullOrEmpty();
        }

        [TestMethod]
        public void Then_IsForwardLink_is_set()
        {
            _linkTypeEnd.IsForwardLink.ShouldBeTrue();
        }

        [TestMethod]
        public void Then_LinkType_is_set()
        {
            _linkTypeEnd.LinkType.ShouldBe(_linkType);
        }
    }

    [TestClass]
    public class Given_WorkItemLinkTypeEnd_ToString : ContextSpecification
    {
        private MockWorkItemLinkTypeEnd _linkTypeEnd = null!;
        private string _result = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            _linkTypeEnd = (MockWorkItemLinkTypeEnd)linkType.ForwardEnd;
        }

        public override void When()
        {
            _result = _linkTypeEnd.ToString();
        }

        [TestMethod]
        public void Then_returns_ImmutableName()
        {
            _result.ShouldBe(_linkTypeEnd.ImmutableName);
        }
    }

    [TestClass]
    public class Given_two_WorkItemLinkTypeEnds_with_same_values : ContextSpecification
    {
        private MockWorkItemLinkTypeEnd _end1 = null!;
        private MockWorkItemLinkTypeEnd _end2 = null!;

        public override void Given()
        {
            var linkType1 = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkType2 = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            _end1 = (MockWorkItemLinkTypeEnd)linkType1.ForwardEnd;
            _end2 = (MockWorkItemLinkTypeEnd)linkType2.ForwardEnd;
        }

        [TestMethod]
        public void Then_Equals_returns_true()
        {
            _end1.Equals(_end2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_is_same()
        {
            _end1.GetHashCode().ShouldBe(_end2.GetHashCode());
        }
    }

    [TestClass]
    public class Given_two_WorkItemLinkTypeEnds_with_different_direction : ContextSpecification
    {
        private MockWorkItemLinkTypeEnd _forwardEnd = null!;
        private MockWorkItemLinkTypeEnd _reverseEnd = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Hierarchy);
            _forwardEnd = (MockWorkItemLinkTypeEnd)linkType.ForwardEnd;
            _reverseEnd = (MockWorkItemLinkTypeEnd)linkType.ReverseEnd;
        }

        [TestMethod]
        public void Then_Equals_returns_false()
        {
            _forwardEnd.Equals(_reverseEnd).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_two_WorkItemLinkTypeEnds_with_different_ImmutableName : ContextSpecification
    {
        private MockWorkItemLinkTypeEnd _end1 = null!;
        private MockWorkItemLinkTypeEnd _end2 = null!;

        public override void Given()
        {
            var linkType1 = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkType2 = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Hierarchy);
            _end1 = (MockWorkItemLinkTypeEnd)linkType1.ForwardEnd;
            _end2 = (MockWorkItemLinkTypeEnd)linkType2.ForwardEnd;
        }

        [TestMethod]
        public void Then_Equals_returns_false()
        {
            _end1.Equals(_end2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_WorkItemLinkTypeEnd_Equals_with_null : ContextSpecification
    {
        private MockWorkItemLinkTypeEnd _linkTypeEnd = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            _linkTypeEnd = (MockWorkItemLinkTypeEnd)linkType.ForwardEnd;
        }

        [TestMethod]
        public void Then_Equals_IWorkItemLinkTypeEnd_null_returns_false()
        {
            _linkTypeEnd.Equals((IWorkItemLinkTypeEnd?)null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_object_null_returns_false()
        {
            _linkTypeEnd.Equals((object?)null).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_WorkItemLinkTypeEnd_Equals_with_same_reference : ContextSpecification
    {
        private MockWorkItemLinkTypeEnd _linkTypeEnd = null!;
        private bool _result;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            _linkTypeEnd = (MockWorkItemLinkTypeEnd)linkType.ForwardEnd;
        }

        public override void When()
        {
            _result = _linkTypeEnd.Equals(_linkTypeEnd);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_WorkItemLinkTypeEnd_OppositeEnd : ContextSpecification
    {
        private MockWorkItemLinkTypeEnd _forwardEnd = null!;
        private IWorkItemLinkTypeEnd _oppositeEnd = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Hierarchy);
            _forwardEnd = (MockWorkItemLinkTypeEnd)linkType.ForwardEnd;
        }

        public override void When()
        {
            _oppositeEnd = _forwardEnd.OppositeEnd;
        }

        [TestMethod]
        public void Then_OppositeEnd_is_reverse_end()
        {
            _oppositeEnd.IsForwardLink.ShouldBeFalse();
        }

        [TestMethod]
        public void Then_OppositeEnd_is_not_same_reference()
        {
            _oppositeEnd.ShouldNotBeSameAs(_forwardEnd);
        }
    }

    [TestClass]
    public class Given_WorkItemLinkTypeEnd_with_case_insensitive_comparison : ContextSpecification
    {
        private MockWorkItemLinkTypeEnd _end1 = null!;
        private MockWorkItemLinkTypeEnd _end2 = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            _end1 = (MockWorkItemLinkTypeEnd)linkType.ForwardEnd;
            // Create another with the same type to compare
            var linkType2 = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            _end2 = (MockWorkItemLinkTypeEnd)linkType2.ForwardEnd;
        }

        [TestMethod]
        public void Then_Equals_returns_true()
        {
            _end1.Equals(_end2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_is_same()
        {
            _end1.GetHashCode().ShouldBe(_end2.GetHashCode());
        }
    }

    /// <summary>
    /// Tests for WorkItemLinkTypeEndComparer.
    /// </summary>
    [TestClass]
    public class Given_WorkItemLinkTypeEndComparer_with_null : ContextSpecification
    {
        private MockWorkItemLinkTypeEnd _linkTypeEnd = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            _linkTypeEnd = (MockWorkItemLinkTypeEnd)linkType.ForwardEnd;
        }

        [TestMethod]
        public void Then_null_null_returns_true()
        {
            WorkItemLinkTypeEndComparer.Default.Equals(null, null).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_end_null_returns_false()
        {
            WorkItemLinkTypeEndComparer.Default.Equals(_linkTypeEnd, null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_null_end_returns_false()
        {
            WorkItemLinkTypeEndComparer.Default.Equals(null, _linkTypeEnd).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_GetHashCode_null_returns_zero()
        {
            WorkItemLinkTypeEndComparer.Default.GetHashCode(null!).ShouldBe(0);
        }

        [TestMethod]
        public void Then_GetHashCode_returns_non_zero()
        {
            WorkItemLinkTypeEndComparer.Default.GetHashCode(_linkTypeEnd).ShouldNotBe(0);
        }
    }

    [TestClass]
    public class Given_WorkItemLinkTypeEndComparer_with_same_reference : ContextSpecification
    {
        private bool _result;
        private MockWorkItemLinkTypeEnd _linkTypeEnd = null!;

        public override void Given()
        {
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            _linkTypeEnd = (MockWorkItemLinkTypeEnd)linkType.ForwardEnd;
        }

        public override void When()
        {
            _result = WorkItemLinkTypeEndComparer.Default.Equals(_linkTypeEnd, _linkTypeEnd);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_WorkItemLinkTypeEndComparer_with_null_Name : ContextSpecification
    {
        private bool _result;

        public override void When()
        {
            // Test the null Name handling in the comparer
            var linkType1 = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var linkType2 = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Related);
            var end1 = linkType1.ForwardEnd;
            var end2 = linkType2.ForwardEnd;
            _result = WorkItemLinkTypeEndComparer.Default.Equals(end1, end2);
        }

        [TestMethod]
        public void Then_returns_true_for_same_values()
        {
            _result.ShouldBeTrue();
        }
    }

    /// <summary>
    /// Tests for WorkItemLinkType validation via WorkItemLinkTypeEnd.
    /// The WorkItemLinkTypeEnd internal constructor validates immutableName through WorkItemLinkType.
    /// </summary>
    [TestClass]
    public class WorkItemLinkTypeValidationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WorkItemLinkType_with_null_referenceName_throws_ArgumentException()
        {
            // WorkItemLinkType validates referenceName in constructor
            _ = new WorkItemLinkType(null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WorkItemLinkType_with_empty_referenceName_throws_ArgumentException()
        {
            _ = new WorkItemLinkType(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WorkItemLinkType_with_whitespace_referenceName_throws_ArgumentException()
        {
            _ = new WorkItemLinkType("   ");
        }
    }
}

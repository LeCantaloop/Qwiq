using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.WorkItemStore.WorkItem
{
    [TestClass]
    public class WorkItemLinkTypeComparerTests : ContextSpecification
    {
        private IWorkItemLinkType _first = null!;

        private IWorkItemLinkType _second = null!;

        private WorkItemLinkTypeComparer _instance = null!;

        private WorkItemLinkTypeEndComparer _instance2 = null!;

        private bool _equalityResult;

        private bool _forwardEqualityResult;

        public override void Given()
        {
            _instance = WorkItemLinkTypeComparer.Default;
            _instance2 = WorkItemLinkTypeEndComparer.Default;

            _first = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Hierarchy);
            _second = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Hierarchy);
        }

        public override void When()
        {
            _equalityResult = _instance.Equals(_first, _second);
            _forwardEqualityResult = _instance2.Equals(_first.ForwardEnd, _second.ForwardEnd);
        }

        [TestMethod]
        public void Different_object_instances_are_equal()
        {
            _equalityResult.ShouldBeTrue();
        }

        [TestMethod]
        public void Different_object_instance_forward_linktypeend_are_equal()
        {
            _forwardEqualityResult.ShouldBeTrue();
        }

        [TestMethod]
        public void Object_HashCodes_are_equal()
        {
            _first.GetHashCode().ShouldEqual(_second.GetHashCode());
        }

        [TestMethod]
        public void Object_and_Comparer_HashCodes_are_equal()
        {
            _first.GetHashCode().ShouldEqual(_instance.GetHashCode(_first));
        }

        [TestMethod]
        public void LinkTypeEnd_Object_HashCodes_are_equal()
        {
            _first.ForwardEnd.GetHashCode().ShouldEqual(_second.ForwardEnd.GetHashCode());
        }

        [TestMethod]
        public void LinkTypeEnd_Object_and_Comparer_HashCodes_are_equal()
        {
            _first.ForwardEnd.GetHashCode().ShouldEqual(_instance2.GetHashCode(_first.ForwardEnd));
        }
    }

    #region WorkItemLinkTypeEndComparer Tests

    /// <summary>
    /// Tests for <see cref="WorkItemLinkTypeEndComparer"/> to verify correct equality behavior,
    /// especially around null handling for the Name property.
    /// </summary>
    [TestClass]
    public class When_comparing_same_link_type_end_reference : ContextSpecification
    {
        private IWorkItemLinkTypeEnd _item = null!;
        private WorkItemLinkTypeEndComparer _comparer = null!;
        private bool _equalityResult;

        public override void Given()
        {
            _comparer = WorkItemLinkTypeEndComparer.Default;
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Hierarchy);
            _item = linkType.ForwardEnd;
        }

        public override void When()
        {
            _equalityResult = _comparer.Equals(_item, _item);
        }

        [TestMethod]
        public void Then_should_return_true()
        {
            _equalityResult.ShouldBeTrue();
        }
    }

    [TestClass]
    public class When_comparing_link_type_end_with_null_first_argument : ContextSpecification
    {
        private IWorkItemLinkTypeEnd _item = null!;
        private WorkItemLinkTypeEndComparer _comparer = null!;
        private bool _equalityResult;

        public override void Given()
        {
            _comparer = WorkItemLinkTypeEndComparer.Default;
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Hierarchy);
            _item = linkType.ForwardEnd;
        }

        public override void When()
        {
            _equalityResult = _comparer.Equals(null, _item);
        }

        [TestMethod]
        public void Then_should_return_false()
        {
            _equalityResult.ShouldBeFalse();
        }
    }

    [TestClass]
    public class When_comparing_link_type_end_with_null_second_argument : ContextSpecification
    {
        private IWorkItemLinkTypeEnd _item = null!;
        private WorkItemLinkTypeEndComparer _comparer = null!;
        private bool _equalityResult;

        public override void Given()
        {
            _comparer = WorkItemLinkTypeEndComparer.Default;
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Hierarchy);
            _item = linkType.ForwardEnd;
        }

        public override void When()
        {
            _equalityResult = _comparer.Equals(_item, null);
        }

        [TestMethod]
        public void Then_should_return_false()
        {
            _equalityResult.ShouldBeFalse();
        }
    }

    [TestClass]
    public class When_comparing_two_null_link_type_ends : ContextSpecification
    {
        private WorkItemLinkTypeEndComparer _comparer = null!;
        private bool _equalityResult;

        public override void Given()
        {
            _comparer = WorkItemLinkTypeEndComparer.Default;
        }

        public override void When()
        {
            _equalityResult = _comparer.Equals(null, null);
        }

        [TestMethod]
        public void Then_should_return_true()
        {
            _equalityResult.ShouldBeTrue();
        }
    }

    [TestClass]
    public class When_comparing_forward_and_reverse_link_type_ends : ContextSpecification
    {
        private IWorkItemLinkTypeEnd _first = null!;
        private IWorkItemLinkTypeEnd _second = null!;
        private WorkItemLinkTypeEndComparer _comparer = null!;
        private bool _equalityResult;

        public override void Given()
        {
            _comparer = WorkItemLinkTypeEndComparer.Default;
            var linkType = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Hierarchy);
            _first = linkType.ForwardEnd;
            _second = linkType.ReverseEnd;
        }

        public override void When()
        {
            _equalityResult = _comparer.Equals(_first, _second);
        }

        [TestMethod]
        public void Then_items_should_not_be_equal()
        {
            _equalityResult.ShouldBeFalse();
        }
    }

    [TestClass]
    public class When_getting_hashcode_of_null_link_type_end : ContextSpecification
    {
        private WorkItemLinkTypeEndComparer _comparer = null!;
        private int _hashCode;

        public override void Given()
        {
            _comparer = WorkItemLinkTypeEndComparer.Default;
        }

        public override void When()
        {
            _hashCode = _comparer.GetHashCode(null!);
        }

        [TestMethod]
        public void Then_should_return_zero()
        {
            _hashCode.ShouldEqual(0);
        }
    }

    [TestClass]
    public class When_comparing_link_type_ends_with_different_ImmutableName : ContextSpecification
    {
        private IWorkItemLinkTypeEnd _first = null!;
        private IWorkItemLinkTypeEnd _second = null!;
        private WorkItemLinkTypeEndComparer _comparer = null!;
        private bool _equalityResult;

        public override void Given()
        {
            _comparer = WorkItemLinkTypeEndComparer.Default;

            var linkType1 = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Hierarchy);
            var linkType2 = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Dependency);

            _first = linkType1.ForwardEnd;
            _second = linkType2.ForwardEnd;
        }

        public override void When()
        {
            _equalityResult = _comparer.Equals(_first, _second);
        }

        [TestMethod]
        public void Then_should_not_be_equal()
        {
            _equalityResult.ShouldBeFalse();
        }
    }

    [TestClass]
    public class When_comparing_identical_link_type_ends_from_different_instances : ContextSpecification
    {
        private IWorkItemLinkTypeEnd _first = null!;
        private IWorkItemLinkTypeEnd _second = null!;
        private WorkItemLinkTypeEndComparer _comparer = null!;
        private bool _equalityResult;

        public override void Given()
        {
            _comparer = WorkItemLinkTypeEndComparer.Default;

            var linkType1 = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Hierarchy);
            var linkType2 = new MockWorkItemLinkType(CoreLinkTypeReferenceNames.Hierarchy);

            _first = linkType1.ForwardEnd;
            _second = linkType2.ForwardEnd;
        }

        public override void When()
        {
            _equalityResult = _comparer.Equals(_first, _second);
        }

        [TestMethod]
        public void Then_should_be_equal()
        {
            _equalityResult.ShouldBeTrue();
        }

        [TestMethod]
        public void Then_hashcodes_should_match()
        {
            _comparer.GetHashCode(_first).ShouldEqual(_comparer.GetHashCode(_second));
        }
    }

    #endregion
}

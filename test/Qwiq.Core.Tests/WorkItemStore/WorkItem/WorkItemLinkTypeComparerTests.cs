using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.WorkItemStore.WorkItem
{
    [TestClass]
    public class WorkItemLinkTypeComparerTests : ContextSpecification
    {
        private IWorkItemLinkType _first;

        private IWorkItemLinkType _second;

        private WorkItemLinkTypeComparer _instance;

        private WorkItemLinkTypeEndComparer _instance2;

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
}

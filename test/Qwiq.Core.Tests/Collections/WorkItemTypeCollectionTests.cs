using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Collections
{
    /// <summary>
    /// Tests for WorkItemTypeCollection Equals and GetHashCode methods.
    /// </summary>
    [TestClass]
    public class Given_WorkItemTypeCollection_with_types : ContextSpecification
    {
        private IWorkItemTypeCollection _collection1 = null!;
        private IWorkItemTypeCollection _collection2 = null!;

        public override void Given()
        {
            _collection1 = new WorkItemTypeCollection(new List<IWorkItemType>
            {
                new MockWorkItemType("Bug"),
                new MockWorkItemType("Task")
            });
            _collection2 = new WorkItemTypeCollection(new List<IWorkItemType>
            {
                new MockWorkItemType("Bug"),
                new MockWorkItemType("Task")
            });
        }

        [TestMethod]
        public void Then_Equals_with_same_types_returns_true()
        {
            _collection1.Equals(_collection2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_Equals_object_with_same_types_returns_true()
        {
            _collection1.Equals((object)_collection2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_is_consistent()
        {
            _collection1.GetHashCode().ShouldBe(_collection1.GetHashCode());
        }

        [TestMethod]
        public void Then_equal_collections_have_same_hash_code()
        {
            _collection1.GetHashCode().ShouldBe(_collection2.GetHashCode());
        }

        [TestMethod]
        public void Then_Equals_with_null_returns_false()
        {
            _collection1.Equals((IWorkItemTypeCollection?)null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_object_with_null_returns_false()
        {
            _collection1.Equals((object?)null).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_WorkItemTypeCollection_with_different_types : ContextSpecification
    {
        private IWorkItemTypeCollection _collection1 = null!;
        private IWorkItemTypeCollection _collection2 = null!;

        public override void Given()
        {
            _collection1 = new WorkItemTypeCollection(new List<IWorkItemType>
            {
                new MockWorkItemType("Bug")
            });
            _collection2 = new WorkItemTypeCollection(new List<IWorkItemType>
            {
                new MockWorkItemType("Task")
            });
        }

        [TestMethod]
        public void Then_Equals_returns_false()
        {
            _collection1.Equals(_collection2).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_hash_codes_are_different()
        {
            // Different types should have different hash codes (usually)
            _collection1.GetHashCode().ShouldNotBe(_collection2.GetHashCode());
        }
    }

    [TestClass]
    public class Given_WorkItemTypeCollection_with_same_reference : ContextSpecification
    {
        private IWorkItemTypeCollection _collection = null!;

        public override void Given()
        {
            _collection = new WorkItemTypeCollection(new List<IWorkItemType>
            {
                new MockWorkItemType("Bug")
            });
        }

        [TestMethod]
        public void Then_Equals_same_reference_returns_true()
        {
            _collection.Equals(_collection).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_Equals_object_same_reference_returns_true()
        {
            _collection.Equals((object)_collection).ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_WorkItemTypeCollection_with_non_collection_object : ContextSpecification
    {
        private IWorkItemTypeCollection _collection = null!;

        public override void Given()
        {
            _collection = new WorkItemTypeCollection(new List<IWorkItemType>
            {
                new MockWorkItemType("Bug")
            });
        }

        [TestMethod]
        public void Then_Equals_object_with_string_returns_false()
        {
            _collection.Equals("not a collection").ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_object_with_int_returns_false()
        {
            _collection.Equals(42).ShouldBeFalse();
        }
    }
}

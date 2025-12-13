using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Collections
{
    /// <summary>
    /// Tests for WorkItemLinkTypeCollection methods.
    /// </summary>
    [TestClass]
    public class Given_WorkItemLinkTypeCollection_with_link_types : ContextSpecification
    {
        private WorkItemLinkTypeCollection _collection1 = null!;
        private WorkItemLinkTypeCollection _collection2 = null!;

        public override void Given()
        {
            // Use different link types with unique forward/reverse end names
            _collection1 = new WorkItemLinkTypeCollection(new List<IWorkItemLinkType>
            {
                new MockWorkItemLinkType("System.LinkTypes.Hierarchy", false, "Child", "Parent", 1),
                new MockWorkItemLinkType("System.LinkTypes.Dependency", false, "Predecessor", "Successor", 2)
            });
            _collection2 = new WorkItemLinkTypeCollection(new List<IWorkItemLinkType>
            {
                new MockWorkItemLinkType("System.LinkTypes.Hierarchy", false, "Child", "Parent", 1),
                new MockWorkItemLinkType("System.LinkTypes.Dependency", false, "Predecessor", "Successor", 2)
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
        public void Then_LinkTypeEnds_returns_collection()
        {
            _collection1.LinkTypeEnds.ShouldNotBeNull();
        }

        [TestMethod]
        public void Then_LinkTypeEnds_contains_all_ends()
        {
            // Non-directional link types only have forward end, so 2 link types = 2 ends
            _collection1.LinkTypeEnds.Count.ShouldBe(2);
        }
    }

    [TestClass]
    public class Given_WorkItemLinkTypeCollection_with_different_types : ContextSpecification
    {
        private WorkItemLinkTypeCollection _collection1 = null!;
        private WorkItemLinkTypeCollection _collection2 = null!;

        public override void Given()
        {
            _collection1 = new WorkItemLinkTypeCollection(new List<IWorkItemLinkType>
            {
                new MockWorkItemLinkType("System.LinkTypes.Related", true, "Related", "Related", 1)
            });
            _collection2 = new WorkItemLinkTypeCollection(new List<IWorkItemLinkType>
            {
                new MockWorkItemLinkType("System.LinkTypes.Dependency", false, "Predecessor", "Successor", 3)
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
            _collection1.GetHashCode().ShouldNotBe(_collection2.GetHashCode());
        }
    }

    [TestClass]
    public class Given_WorkItemLinkTypeCollection_with_null : ContextSpecification
    {
        private WorkItemLinkTypeCollection _collection = null!;

        public override void Given()
        {
            _collection = new WorkItemLinkTypeCollection(new List<IWorkItemLinkType>
            {
                new MockWorkItemLinkType("System.LinkTypes.Related", true, "Related", "Related", 1)
            });
        }

        [TestMethod]
        public void Then_Equals_with_null_returns_false()
        {
            _collection.Equals((IWorkItemLinkTypeCollection?)null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_object_with_null_returns_false()
        {
            _collection.Equals((object?)null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_object_with_non_collection_returns_false()
        {
            _collection.Equals("not a collection").ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_WorkItemLinkTypeCollection_same_reference : ContextSpecification
    {
        private WorkItemLinkTypeCollection _collection = null!;

        public override void Given()
        {
            _collection = new WorkItemLinkTypeCollection(new List<IWorkItemLinkType>
            {
                new MockWorkItemLinkType("System.LinkTypes.Related", true, "Related", "Related", 1)
            });
        }

        [TestMethod]
        public void Then_Equals_same_reference_returns_true()
        {
            _collection.Equals((object)_collection).ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_WorkItemLinkTypeCollection_with_directional_types : ContextSpecification
    {
        private WorkItemLinkTypeCollection _collection = null!;

        public override void Given()
        {
            // Use directional link types (isDirectional=true) which have both forward and reverse ends
            _collection = new WorkItemLinkTypeCollection(new List<IWorkItemLinkType>
            {
                new MockWorkItemLinkType("System.LinkTypes.Hierarchy", true, "Child", "Parent", 1)
            });
        }

        [TestMethod]
        public void Then_LinkTypeEnds_contains_both_forward_and_reverse()
        {
            // Directional link type has both forward and reverse ends
            _collection.LinkTypeEnds.Count.ShouldBe(2);
        }

        [TestMethod]
        public void Then_LinkTypeEnds_can_be_accessed_by_name()
        {
            _collection.LinkTypeEnds["Child"].ShouldNotBeNull();
            _collection.LinkTypeEnds["Parent"].ShouldNotBeNull();
        }
    }

    [TestClass]
    public class Given_WorkItemLinkTypeCollection_with_empty_list : ContextSpecification
    {
        private WorkItemLinkTypeCollection _collection = null!;

        public override void Given()
        {
            _collection = new WorkItemLinkTypeCollection(new List<IWorkItemLinkType>());
        }

        [TestMethod]
        public void Then_GetHashCode_returns_seed_value()
        {
            // Empty collection should return the initial seed value of 27
            _collection.GetHashCode().ShouldBe(27);
        }

        [TestMethod]
        public void Then_LinkTypeEnds_is_empty()
        {
            _collection.LinkTypeEnds.Count.ShouldBe(0);
        }
    }
}

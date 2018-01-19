using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;
using System;
using System.Linq;

namespace Microsoft.Qwiq.Mapper
{
    [TestClass]
    public class
        Given_an_AttributeMapper_with_a_WorkItemStore_DefaultFields_without_TeamProject_and_WorkItemType_specified :
            WiqlAttributeMapperContextSpecification
    {
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        public void The_work_items_are_mapped_to_their_model()
        {
            Bugs.ToList().Count.ShouldEqual(1);
        }

        protected override void ConfigureOptions()
        {
            WorkItemStore.Configuration.DefaultFields = new[]
            {
                CoreFieldRefNames.Id,
                CoreFieldRefNames.State
            };
        }
    }

    [TestClass]
    public class
        Given_an_AttributeMapper_with_a_WorkItemStore_DefaultFields_without_TeamProject_and_WorkItemType_specified_Eager :
            Given_an_AttributeMapper_with_a_WorkItemStore_DefaultFields_without_TeamProject_and_WorkItemType_specified
    {
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        public new void The_work_items_are_mapped_to_their_model()
        {
            Bugs.ToList().Count.ShouldEqual(1);
        }

        protected override void ConfigureOptions()
        {
            base.ConfigureOptions();
            WorkItemStore.Configuration.LazyLoadingEnabled = false;
        }
    }

    [TestClass]
    public class
        Given_an_AttributeMapper_with_a_WorkItemStore_DefaultFields_without_WorkItemType_specified :
            WiqlAttributeMapperContextSpecification
    {
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        public void The_work_items_are_mapped_to_their_model()
        {
            Bugs.ToList().Count.ShouldEqual(1);
        }

        protected override void ConfigureOptions()
        {
            WorkItemStore.Configuration.DefaultFields = new[]
            {
                CoreFieldRefNames.TeamProject,
                CoreFieldRefNames.Id,
                CoreFieldRefNames.State
            };
        }
    }
    [TestClass]
    public class
        Given_an_AttributeMapper_with_a_WorkItemStore_DefaultFields_without_WorkItemType_specified_Eager :
            Given_an_AttributeMapper_with_a_WorkItemStore_DefaultFields_without_WorkItemType_specified
    {
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        public new void The_work_items_are_mapped_to_their_model()
        {
            Bugs.ToList().Count.ShouldEqual(1);
        }

        protected override void ConfigureOptions()
        {
            base.ConfigureOptions();
            WorkItemStore.Configuration.LazyLoadingEnabled = false;
        }
    }
}
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

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
        [ExpectedException(typeof(InvalidOperationException))]
        public void An_InvalidOperationException_is_thrown()
        {
            Bugs.ToList().Count.ShouldEqual(1);
        }

        protected override void ConfigureOptions()
        {
            WorkItemStore.Configuration.DefaultFields = new[]
            {
                //CoreFieldRefNames.TeamProject,
                //CoreFieldRefNames.WorkItemType,
                CoreFieldRefNames.Id,
                CoreFieldRefNames.State
            };
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void An_InvalidOperationException_is_thrown()
        {
            Bugs.ToList().Count.ShouldEqual(1);
        }

        protected override void ConfigureOptions()
        {
            WorkItemStore.Configuration.DefaultFields = new[]
            {
                CoreFieldRefNames.TeamProject,
                //CoreFieldRefNames.WorkItemType,
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
        protected override void ConfigureOptions()
        {
            base.ConfigureOptions();
            WorkItemStore.Configuration.LazyLoadingEnabled = false;
        }
    }

    [TestClass]
    public class
        Given_an_AttributeMapper_with_a_WorkItemStore_DefaultFields_without_WorkItemType_specified_Eager :
            Given_an_AttributeMapper_with_a_WorkItemStore_DefaultFields_without_WorkItemType_specified
    {


        protected override void ConfigureOptions()
        {
            base.ConfigureOptions();
            WorkItemStore.Configuration.LazyLoadingEnabled = false;
        }
    }
}

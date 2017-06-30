using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.WorkItemStore
{
    [TestClass]
    public class LargeWiqlHierarchyQueryTests : LargeHierarchyContextSpecification
    {
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Links_Equal()
        {
            RestResult.Links.ShouldContainOnly(SoapResult.Links);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void WorkItems_Equal()
        {
            RestResult.WorkItems.ShouldContainOnly(SoapResult.WorkItems);
        }
    }

    [TestClass]
    public class LargeWiqlHierarchyQueryTests_EagerLoad : LargeWiqlHierarchyQueryTests
    {
        /// <inheritdoc/>
        ///
        public override void Given()
        {
            base.Given();
            Soap.Configuration.LazyLoadingEnabled = false;
            Rest.Configuration.LazyLoadingEnabled = false;
        }
    }

    [TestClass]
    public class LargeWiqlHierarchyQueryTests_EagerLoad_NoFieldExpansion : LargeWiqlHierarchyQueryTests_EagerLoad
    {
        /// <inheritdoc/>
        ///
        public override void Given()
        {
            base.Given();

            Soap.Configuration.WorkItemExpand = WorkItemExpand.None;
            Rest.Configuration.WorkItemExpand = WorkItemExpand.None;
        }
    }
}
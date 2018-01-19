using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.WorkItemStore.WorkItem
{
    [TestClass]
    public class Given_a_WorkItem_with_Links : WorkItemWithLinksContextSpecification
    {
        /// <inheritdoc />
        public override void Given()
        {
            base.Given();
            Rest.Configuration.WorkItemExpand = WorkItemExpand.All;
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        [ExpectedException(typeof(NotSupportedException))]
        [Ignore]
        public void AttachedFileCount_is_equal()
        {
            AssertWorkItemExpandConfiguration();
            RestResult.WorkItem.AttachedFileCount.ShouldEqual(SoapResult.WorkItem.AttachedFileCount);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        [Ignore]
        public void ExternalLinkCount_is_equal()
        {
            AssertWorkItemExpandConfiguration();
            RestResult.WorkItem.ExternalLinkCount.ShouldEqual(SoapResult.WorkItem.ExternalLinkCount);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        [Ignore]
        public void HyperlinkCount_is_equal()
        {
            AssertWorkItemExpandConfiguration();
            RestResult.WorkItem.HyperlinkCount.ShouldEqual(SoapResult.WorkItem.HyperlinkCount);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        [Ignore]
        public void links_from_both_implementations_are_equal()
        {
            AssertWorkItemExpandConfiguration();

            RestResult.WorkItem.Links.ShouldContainOnly(SoapResult.WorkItem.Links);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        [Ignore]
        public void RelatedLinkCount_is_equal()
        {
            AssertWorkItemExpandConfiguration();
            RestResult.WorkItem.RelatedLinkCount.ShouldEqual(SoapResult.WorkItem.RelatedLinkCount);
        }

        private void AssertWorkItemExpandConfiguration()
        {
            if (RestResult.WorkItemStore.Configuration.WorkItemExpand == WorkItemExpand.None
                || RestResult.WorkItemStore.Configuration.WorkItemExpand == WorkItemExpand.Fields)
                Assert.Inconclusive("The links could not tested because the expand configuration was not set to include links.");
        }
    }
}
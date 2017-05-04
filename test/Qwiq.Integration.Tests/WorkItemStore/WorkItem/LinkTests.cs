using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
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
            ((Client.Rest.WorkItemStoreConfiguration)Rest.Configuration).WorkItemExpand = WorkItemExpand.Relations;
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        [ExpectedException(typeof(NotSupportedException))]
        public void AttachedFileCount_is_equal()
        {
            AssertWorkItemExpandConfiguration();
            RestResult.WorkItem.AttachedFileCount.ShouldEqual(SoapResult.WorkItem.AttachedFileCount);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        public void ExternalLinkCount_is_equal()
        {
            AssertWorkItemExpandConfiguration();
            RestResult.WorkItem.ExternalLinkCount.ShouldEqual(SoapResult.WorkItem.ExternalLinkCount);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        public void HyperlinkCount_is_equal()
        {
            AssertWorkItemExpandConfiguration();
            RestResult.WorkItem.HyperlinkCount.ShouldEqual(SoapResult.WorkItem.HyperlinkCount);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        public void links_from_both_implementations_are_equal()
        {
            AssertWorkItemExpandConfiguration();

            RestResult.WorkItem.Links.ShouldContainOnly(SoapResult.WorkItem.Links);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        public void RelatedLinkCount_is_equal()
        {
            AssertWorkItemExpandConfiguration();
            RestResult.WorkItem.RelatedLinkCount.ShouldEqual(SoapResult.WorkItem.RelatedLinkCount);
        }

        private void AssertWorkItemExpandConfiguration()
        {
            if (((Client.Rest.WorkItemStoreConfiguration)RestResult.WorkItemStore.Configuration).WorkItemExpand != WorkItemExpand.Links)
                Assert.Inconclusive("The links could not tested because the expand configuration was not set.");
        }
    }
}
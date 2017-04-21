using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Integration.Tests
{
    [TestClass]
    public class Given_a_WorkItem_with_Links : WorkItemWithLinksContextSpecification
    {
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        public void links_from_both_implementations_are_equal()
        {
            RestResult.WorkItem.Links.ShouldContainOnly(SoapResult.WorkItem.Links);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        public void ExternalLinkCount_is_equal()
        {
            RestResult.WorkItem.ExternalLinkCount.ShouldEqual(SoapResult.WorkItem.ExternalLinkCount);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        public void HyperlinkCount_is_equal()
        {
            RestResult.WorkItem.HyperlinkCount.ShouldEqual(SoapResult.WorkItem.HyperlinkCount);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        [ExpectedException(typeof(NotSupportedException))]
        public void AttachedFileCount_is_equal()
        {
            RestResult.WorkItem.AttachedFileCount.ShouldEqual(SoapResult.WorkItem.AttachedFileCount);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        public void RelatedLinkCount_is_equal()
        {
            RestResult.WorkItem.RelatedLinkCount.ShouldEqual(SoapResult.WorkItem.RelatedLinkCount);
        }
    }

    public abstract class WorkItemWithLinksContextSpecification : WorkItemStoreComparisonContextSpecification
    {
        private const int Id = 104268;

        public override void When()
        {
            SoapResult.WorkItem = TimedAction(() => SoapResult.WorkItemStore.Query(Id), "SOAP", "Query By Id");
            RestResult.WorkItem = TimedAction(() => RestResult.WorkItemStore.Query(Id), "REST", "Query By Id");

            SoapResult.WorkItem.ExternalLinkCount.ShouldBeGreaterThan(0);
            SoapResult.WorkItem.HyperlinkCount.ShouldBeGreaterThan(0);
            SoapResult.WorkItem.AttachedFileCount.ShouldBeGreaterThan(0);
            SoapResult.WorkItem.RelatedLinkCount.ShouldBeGreaterThan(0);
        }
    }
}
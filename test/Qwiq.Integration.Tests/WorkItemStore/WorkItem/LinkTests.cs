using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.WorkItemStore.WorkItem
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

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void SOAP_WorkItem_has_External_links()
        {
            AssertWorkItemExpandConfiguration();

            SoapResult.WorkItem.ExternalLinkCount.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void SOAP_WorkItem_has_Hyper_links()
        {
            AssertWorkItemExpandConfiguration();
            SoapResult.WorkItem.HyperlinkCount.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void SOAP_WorkItem_has_Attached_files()
        {
            AssertWorkItemExpandConfiguration();

            // Sandbox work item MUST have attachments configured for this test to pass
            // Run scripts/Validate-SandboxEnvironment.ps1 to verify environment configuration
            if (SoapResult.WorkItem.AttachedFileCount == 0)
            {
                Assert.Fail(
                    $"Test work item {TestData.WorkItemWithLinksId} has no attachments. " +
                    "Add at least one attachment to this work item in the sandbox environment. " +
                    "Run 'scripts/Validate-SandboxEnvironment.ps1' to verify environment configuration.");
            }

            SoapResult.WorkItem.AttachedFileCount.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void SOAP_WorkItem_has_Related_links()
        {
            AssertWorkItemExpandConfiguration();

            SoapResult.WorkItem.RelatedLinkCount.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        public void REST_WorkItem_has_External_links()
        {
            AssertWorkItemExpandConfiguration();

            RestResult.WorkItem.ExternalLinkCount.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        public void REST_WorkItem_has_Hyper_links()
        {
            AssertWorkItemExpandConfiguration();

            RestResult.WorkItem.HyperlinkCount.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [ExpectedException(typeof(NotSupportedException))]
        public void REST_WorkItem_has_Attached_files()
        {
            AssertWorkItemExpandConfiguration();

            RestResult.WorkItem.AttachedFileCount.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        public void REST_WorkItem_has_Related_links()
        {
            AssertWorkItemExpandConfiguration();

            RestResult.WorkItem.RelatedLinkCount.ShouldBeGreaterThan(0);
        }

        private void AssertWorkItemExpandConfiguration()
        {
            // This is a test setup issue, not an environment data issue.
            // The Given() method should configure WorkItemExpand appropriately.
            if (RestResult.WorkItemStore.Configuration.WorkItemExpand == WorkItemExpand.None
                || RestResult.WorkItemStore.Configuration.WorkItemExpand == WorkItemExpand.Fields)
            {
                Assert.Fail(
                    "Test setup error: WorkItemExpand must be set to include links (All or Relations). " +
                    $"Current value: {RestResult.WorkItemStore.Configuration.WorkItemExpand}. " +
                    "Ensure the Given() method sets Rest.Configuration.WorkItemExpand = WorkItemExpand.All.");
            }
        }
    }
}
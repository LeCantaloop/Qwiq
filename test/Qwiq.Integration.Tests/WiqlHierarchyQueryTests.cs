using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Integration.Tests
{
    [TestClass]
    public class WiqlHierarchyQueryTests : WorkItemStoreComparisonContextSpecification
    {
        protected Result RestResult { get; set; }

        protected Result SoapResult { get; set; }

        public override void Given()
        {
            base.Given();

            SoapResult = new Result() { WorkItemStore = Soap };
            RestResult = new Result() { WorkItemStore = Rest };
        }

        public override void When()
        {
            const string WIQL = @"
SELECT *
FROM WorkItemLinks
WHERE 
    [Source].[System.TeamProject] = 'OS' AND
    [Source].[System.ID] = 10726528 AND
    [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward' AND
    [Target].[System.WorkItemType] = 'Scenario'
mode(recursive)
";

            RestResult.WorkItemLinks = TimedAction(() => RestResult.WorkItemStore.QueryLinks(WIQL).ToList(), "REST", "QueryLinks");
            SoapResult.WorkItemLinks = TimedAction(() => SoapResult.WorkItemStore.QueryLinks(WIQL).ToList(), "SOAP", "QueryLinks");
        }

        public override void Cleanup()
        {
            SoapResult?.Dispose();
            RestResult?.Dispose();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void SOAP_Links_returned()
        {
            SoapResult.WorkItemLinks.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void REST_Links_returned()
        {
            RestResult.WorkItemLinks.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Same_number_of_links_returned()
        {
            RestResult.WorkItemLinks.Count().ShouldEqual(SoapResult.WorkItemLinks.Count());
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void WorkItemLink_SourceId_TargetId_are_equal()
        {
            RestResult.WorkItemLinks.ShouldContainOnly(SoapResult.WorkItemLinks);
        }

        protected class Result : IDisposable
        {
            public IWorkItem WorkItem { get; set; }

            public IEnumerable<IWorkItemLinkInfo> WorkItemLinks { get; set; }

            public IWorkItemStore WorkItemStore { get; set; }

            public void Dispose()
            {
                WorkItemStore?.Dispose();
            }
        }
    }
}
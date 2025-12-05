using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.WorkItemStore
{
    [TestClass]
    public class WiqlHierarchyQueryTests : WorkItemStoreComparisonContextSpecification
    {
        public override void When()
        {
            // Use TestData.HierarchyParentId (User Story with 2 Task children)
            // Note: Target type changed from 'Scenario' to 'Task' to match sandbox data
            var wiql = $@"
SELECT *
FROM WorkItemLinks
WHERE
    [Source].[System.TeamProject] = '{TestData.ProjectName}' AND
    [Source].[System.ID] = {TestData.HierarchyParentId} AND
    [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward' AND
    [Target].[System.WorkItemType] = 'Task'
mode(recursive)
";

            RestResult.Links = TimedAction(() => RestResult.WorkItemStore!.QueryLinks(wiql).ToList(), "REST", "QueryLinks");
            SoapResult.Links = TimedAction(() => SoapResult.WorkItemStore!.QueryLinks(wiql).ToList(), "SOAP", "QueryLinks");
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void SOAP_Links_returned()
        {
            SoapResult.Links!.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void REST_Links_returned()
        {
            RestResult.Links!.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Same_number_of_links_returned()
        {
            RestResult.Links!.Count().ShouldEqual(SoapResult.Links!.Count());
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void WorkItemLink_SourceId_TargetId_are_equal()
        {
            RestResult.Links!.ShouldContainOnly(SoapResult.Links!);
        }
    }
}
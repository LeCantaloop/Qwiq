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

            RestResult.Links = TimedAction(() => RestResult.WorkItemStore.QueryLinks(WIQL).ToList(), "REST", "QueryLinks");
            SoapResult.Links = TimedAction(() => SoapResult.WorkItemStore.QueryLinks(WIQL).ToList(), "SOAP", "QueryLinks");
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void SOAP_Links_returned()
        {
            SoapResult.Links.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void REST_Links_returned()
        {
            RestResult.Links.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Same_number_of_links_returned()
        {
            RestResult.Links.Count().ShouldEqual(SoapResult.Links.Count());
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void WorkItemLink_SourceId_TargetId_are_equal()
        {
            RestResult.Links.ShouldContainOnly(SoapResult.Links);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TeamFoundation.WorkItemTracking.Common.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Core.Tests
{
    [TestClass]
    public class SingleIdTests : IntegrationContextSpecification
    {
        private const int Id = 10726528;

        public override void When()
        {
            SoapResult.WorkItem = SoapResult.WorkItemStore.Query(Id);
            RestResult.WorkItem = RestResult.WorkItemStore.Query(Id);
        }
    }

    [TestClass]
    public class MultipleIdTests : IntegrationContextSpecification
    {
        private const int Id = 10726528;

        public override void When()
        {
            SoapResult.WorkItem = SoapResult.WorkItemStore.Query(new[] { Id }).Single();
            RestResult.WorkItem = RestResult.WorkItemStore.Query(new[] { Id }).Single();
        }
    }

    [TestClass]
    public class WiqlFlatQueryTests : IntegrationContextSpecification
    {
        private const int Id = 10726528;

        private static readonly string Wiql = $"SELECT {string.Join(", ", CoreFields)} FROM WorkItems WHERE [System.Id] = {Id}";

        public override void When()
        {
            RestResult.WorkItem = RestResult.WorkItemStore.Query(Wiql).Single();
            SoapResult.WorkItem = SoapResult.WorkItemStore.Query(Wiql).Single();
        }
    }

    [TestClass]
    public class LargeWiqlHierarchyQueryTests : ContextSpecification
    {
        protected Result RestResult { get; set; }

        protected Result SoapResult { get; set; }

        public override void Given()
        {
            var credentials = Credentials.CredentialsFactory.CreateCredentials((string)null);
            var fac = WorkItemStoreFactory.GetInstance();
            var uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

            SoapResult = new Result() { WorkItemStore = fac.Create(uri, credentials, ClientType.Soap) };
            RestResult = new Result() { WorkItemStore = fac.Create(uri, credentials, ClientType.Rest) };
        }

        public override void When()
        {
            const string WIQL = @"
SELECT [Microsoft.VSTS.CMMI.TaskType], [System.Reason], [System.Id], [Priority], [OSG.Rank], [Microsoft.VSTS.Scheduling.OriginalEstimate], [OSG.QualityEstimate], [OSG.DevEstimate], [OSG.Cost], [OSG.RemainingDays], [System.Title], [System.State], [Microsoft.VSTS.Common.Release], [Microsoft.VSTS.Common.Keywords], [System.IterationPath], [System.AreaPath], [System.WorkItemType], [OSG.Product]
FROM WorkItemLinks
WHERE
    Source.[System.Id] IN (164972, 306417, 710095, 3135398, 3249369, 3707824, 4233618, 4708876, 4774628, 4774633, 4858832, 5050537, 5080053, 5321076, 5347847, 5348414, 5399017, 5399125, 5399142, 5404813, 5503336, 5570261, 5580845, 5639025, 5662931, 5674580, 5755004, 5779431, 5791052, 5810289, 6251354, 6418471, 6420861, 6966852, 7138009, 7266472, 7358985, 7417629, 7418003, 7418008, 7475634, 7529329, 7547051, 7547912, 7578388, 7710163, 7726674, 7734221, 7734222, 7734223, 7734225, 7734226, 7734229, 7734230, 7734231, 7734232, 7734234, 7734239, 7734240, 7734242, 7734245, 7734246, 7734247, 7734252, 7734254, 7734255, 7734258, 7734260, 7734261, 7734262, 7734263, 7734264, 7734267, 7734268, 7734269, 7762780, 7802385, 7825004, 7879923, 7881302, 7889153, 7899054, 7899115, 7899207, 7899214, 7899226, 7924439, 7925256, 7925264, 7925414, 7925435, 7978728, 7989044, 7989565, 8008196, 8029597, 8062462, 8062582, 8062597, 8083468, 8112721, 8150555, 8165162, 8165174, 8165211, 8167621, 8167748, 8168041, 8168698, 8169478, 8169633, 8181048, 8217827, 8231545, 8232318, 8240155, 8249590, 8249712, 8269111, 8273089, 8273096, 8273195, 8291862, 8291899, 8305945, 8311776, 8328982, 8329001, 8329069, 8329093, 8329134, 8329157, 8329534, 8329644, 8329835, 8330248, 8339386, 8339426, 8339444, 8340276, 8402025, 8403861, 8415967, 8461788, 8461811, 8464765, 8478429, 8478988, 8479926, 8511916, 8521180, 8523040, 8525637, 8550287, 8584490, 8629612, 8629628, 8647508, 8704880, 8771767, 8771853, 8889742, 8974486, 8975430, 8975466, 8975520, 8975657, 9043259, 9197486, 9197751, 9202677, 9204728, 9279773, 9280064, 9403077, 9403332, 9403384, 9403462, 9405201, 9406323, 9411986, 9416107, 9416388, 9416413, 9416498, 9416502, 9416513, 9416557, 9416562, 9416600, 9416619, 9416637, 9416710, 9416724, 9416744, 9416752, 9416768, 9416800, 9445547, 9457647, 9459369, 9459387, 9459443, 9460109, 9460216, 9460225, 9460246, 9460256, 9460263, 9492546, 9498278, 9506510, 9506522, 9506571, 9506641, 9637927, 9644862, 9647371, 9718740, 9780582, 9796760, 9810502, 9831841, 9836943, 10013800, 10033936, 10211640, 10277871, 10397497, 10429016, 10448096, 10471271, 10471325, 10513555, 10527610, 10530029, 10530447, 10530854, 10562556, 10588201, 10597347, 10597392, 10598841, 10608217, 10621199, 10621202, 10621236, 10621302, 10650244, 10693987, 10694717, 10696544, 10696590, 10696618, 10696638, 10726461, 10726528, 10726538, 10726549, 10726569, 10726584, 10726595, 10726623, 10726641, 10731466, 10732765, 10735376, 10741454, 10742199, 10748682, 10753466, 10754027, 10760613, 10760657, 10760716, 10760745, 10760797, 10761005, 10764882, 10769936, 10769985, 10770155, 10779176, 10779780, 10780103, 10780404, 10781600, 10781696, 10782702, 10783011, 10784489, 10786336, 10786381, 10788814, 10788938, 10788942, 10788980, 10788991, 10789027, 10789081, 10789087, 10789109, 10789112, 10789116, 10789118, 10789122, 10789177, 10789263, 10789737, 10795841, 10796505, 10797459, 10799250, 10799746, 10801935, 10802569, 10803778, 10804358, 10804374, 10804396, 10804611, 10804706, 10824786, 10824794, 10824844, 10825656, 10835610, 10835947, 10836042, 10836468, 10838442, 10838469, 10838487, 10847341, 10851434, 10870376, 10870428, 10870517, 10871100, 10872787, 10872818, 10872890, 10872906, 10872957, 10872990)
    AND [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'
    AND Target.[System.AreaPath] UNDER 'OS\Core\WebPlat'
    AND Target.[System.WorkItemType] IN ('Customer Promise', 'Scenario', 'Measure', 'Deliverable', 'Task')    
mode(Recursive)
";

            RestResult.WorkItemLinks = RestResult.WorkItemStore.QueryLinks(WIQL).ToList();
            SoapResult.WorkItemLinks = SoapResult.WorkItemStore.QueryLinks(WIQL).ToList();
        }

        public override void Cleanup()
        {
            SoapResult?.Dispose();
            RestResult?.Dispose();
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

        [TestMethod]
        [TestCategory("localOnly")]
        public void BigTest()
        {
        }
    }

    [TestClass]
    public class WiqlHierarchyQueryTests : ContextSpecification
    {
        protected Result RestResult { get; set; }

        protected Result SoapResult { get; set; }

        public override void Given()
        {
            var credentials = Credentials.CredentialsFactory.CreateCredentials((string)null);
            var fac = WorkItemStoreFactory.GetInstance();
            var uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

            SoapResult = new Result() { WorkItemStore = fac.Create(uri, credentials, ClientType.Soap) };
            RestResult = new Result() { WorkItemStore = fac.Create(uri, credentials, ClientType.Rest) };
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

            RestResult.WorkItemLinks = RestResult.WorkItemStore.QueryLinks(WIQL).ToList();
            SoapResult.WorkItemLinks = SoapResult.WorkItemStore.QueryLinks(WIQL).ToList();
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
            for (var i = 0; i < RestResult.WorkItemLinks.Count(); i++)
            {
                var r = RestResult.WorkItemLinks.ElementAt(i);
                var s = SoapResult.WorkItemLinks.ElementAt(i);

                r.SourceId.ShouldEqual(s.SourceId);
                r.TargetId.ShouldEqual(s.TargetId);
            }
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


    public abstract class IntegrationContextSpecification : ContextSpecification
    {
        protected Result RestResult { get; set; }

        protected Result SoapResult { get; set; }

        [TestMethod]
        [TestCategory("localOnly")]
        public void AreaPath_is_equal()
        {
            RestResult.WorkItem.AreaPath.ShouldEqual(SoapResult.WorkItem.AreaPath);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void AssignedTo_is_equal()
        {
            RestResult.WorkItem.AssignedTo.ShouldEqual(SoapResult.WorkItem.AssignedTo);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void ChangedBy_is_equal()
        {
            RestResult.WorkItem.ChangedBy.ShouldEqual(SoapResult.WorkItem.ChangedBy);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void ChangedDate_is_equal()
        {
            RestResult.WorkItem.ChangedDate.ShouldEqual(SoapResult.WorkItem.ChangedDate.ToLocalTime());
        }

        public override void Cleanup()
        {
            SoapResult?.Dispose();
            RestResult?.Dispose();
        }

        protected static readonly string[] CoreFields =
                {
                    "System.AreaPath", "System.AssignedTo", "System.AttachedFileCount", "System.ChangedBy",
                    "System.ChangedDate", "System.CreatedBy", "System.CreatedDate", "System.Description",
                    "System.ExternalLinkCount", "System.History", "System.HyperLinkCount", "System.Id",
                    "System.IterationPath", "System.RelatedLinkCount", "System.Rev", "System.RevisedDate",
                    "System.State", "System.Title", "System.WorkItemType", CoreFieldRefNames.Tags
                };

        [TestMethod]
        [TestCategory("localOnly")]
        public void CoreFields_are_equal()
        {
            foreach (var field in CoreFields)
            {
                RestResult.WorkItem[field].ShouldEqual(SoapResult.WorkItem[field], field);
            }
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void CreatedBy_is_equal()
        {
            RestResult.WorkItem.CreatedBy.ShouldEqual(SoapResult.WorkItem.CreatedBy);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Rev_is_equal()
        {
            RestResult.WorkItem.Rev.ShouldEqual(SoapResult.WorkItem.Rev);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void CreatedDate_is_equal()
        {
            RestResult.WorkItem.CreatedDate.ShouldEqual(SoapResult.WorkItem.CreatedDate);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Description_is_equal()
        {
            RestResult.WorkItem.Description.ShouldEqual(SoapResult.WorkItem.Description);
        }

        public override void Given()
        {
            var credentials = Credentials.CredentialsFactory.CreateCredentials((string)null);
            var fac = WorkItemStoreFactory.GetInstance();
            var uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

            SoapResult = new Result() { WorkItemStore = fac.Create(uri, credentials, ClientType.Soap) };
            RestResult = new Result() { WorkItemStore = fac.Create(uri, credentials, ClientType.Rest) };
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void History_is_equal()
        {
            RestResult.WorkItem.History.ShouldEqual(SoapResult.WorkItem.History);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Id_is_equal()
        {
            RestResult.WorkItem.Id.ShouldEqual(SoapResult.WorkItem.Id);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void IterationPath_is_equal()
        {
            RestResult.WorkItem.IterationPath.ShouldEqual(SoapResult.WorkItem.IterationPath);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void REST_WorkItem_is_returned()
        {
            RestResult.WorkItem.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void SOAP_WorkItem_is_returned()
        {
            SoapResult.WorkItem.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void State_is_equal()
        {
            RestResult.WorkItem.State.ShouldEqual(SoapResult.WorkItem.State);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Tags_is_equal()
        {
            RestResult.WorkItem.Tags.ShouldEqual(SoapResult.WorkItem.Tags);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Title_is_equal()
        {
            RestResult.WorkItem.Title.ShouldEqual(SoapResult.WorkItem.Title);
        }

        protected class Result : IDisposable
        {
            public IWorkItem WorkItem { get; set; }

            public IEnumerable<IWorkItemLinkInfo> Links { get; set; }

            public IWorkItemStore WorkItemStore { get; set; }

            public void Dispose()
            {
                WorkItemStore?.Dispose();
            }
        }
    }
}
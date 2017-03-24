using System;
using System.Collections.Generic;
using System.Diagnostics;
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
SELECT [System.Id]
FROM WorkItemLinks
WHERE
    Source.[System.Id] IN (306417, 3135398, 3249369, 3492942, 4233618, 4708876, 4774628, 4774633, 5081979, 5145876, 5321076, 5347847, 5348414, 5399017, 5399125, 5399142, 5437042, 5580845, 5696165, 5755004, 5779431, 5791052, 5810594, 5814045, 6190723, 6340710, 6418471, 6420861, 6507248, 6966852, 7147489, 7358985, 7418008, 7462755, 7529329, 7547051, 7547912, 7578388, 7726674, 7734222, 7734226, 7734230, 7734245, 7734252, 7734254, 7734255, 7734260, 7734261, 7734269, 7802385, 7825004, 7881302, 7899054, 7899207, 7899214, 7899226, 7925414, 7978728, 7989565, 8040116, 8062597, 8083468, 8122369, 8168698, 8169478, 8169633, 8172195, 8219936, 8233267, 8240155, 8249707, 8256930, 8256939, 8256943, 8256947, 8273089, 8273096, 8274155, 8285368, 8291862, 8305945, 8311776, 8329534, 8402025, 8415967, 8461788, 8461811, 8464765, 8478988, 8479926, 8480154, 8480258, 8521180, 8523040, 8584490, 8647508, 8704880, 8909592, 9067373, 9197486, 9197751, 9256654, 9279773, 9403332, 9405201, 9416107, 9445547, 9451633, 9492546, 9637927, 9644862, 9647371, 9780582, 9796760, 9831841, 9836943, 9853813, 9941648, 10397497, 10397521, 10398675, 10448096, 10471325, 10499131, 10513555, 10527610, 10530854, 10552224, 10562556, 10588201, 10597347, 10597392, 10621199, 10621202, 10629409, 10650244, 10693987, 10696618, 10726461, 10726528, 10726538, 10726549, 10726569, 10726584, 10726595, 10726623, 10726641, 10731011, 10732765, 10748682, 10753466, 10754027, 10757138, 10760613, 10760657, 10760716, 10760745, 10760797, 10761005, 10762808, 10764882, 10769936, 10769966, 10769985, 10770045, 10770155, 10772744, 10781600, 10781696, 10782702, 10783490, 10784489, 10785622, 10786336, 10786381, 10788814, 10788938, 10789724, 10789737, 10795841, 10796505, 10796527, 10796531, 10796651, 10797459, 10799250, 10799746, 10802354, 10802569, 10803640, 10803778, 10804396, 10804706, 10825656, 10833432, 10834702, 10835610, 10835947, 10854312, 10872890, 10872990, 10875938, 10887469, 10888050, 10889671, 10893905, 10919153, 10940803, 10940927, 10955448, 10955522, 10956000, 10956061, 10956625, 10961487, 10961734, 10961908, 10983282, 10984200, 10991608, 11013506, 11017539, 11018019, 11018123, 11018139, 11018157, 11018183, 11018222, 11018336, 11053481, 11057194, 11070906, 11076205, 11084268, 11084305, 11084326, 11094144, 11102448, 11102465, 11102485, 11116880, 11126107, 11126919, 11127051, 11129966, 11163126, 11175208, 11175250, 11175307, 11175332, 11175459, 11178801, 11184570, 11193519, 11194902, 11197556, 11205742, 11240677, 11240737, 11275343, 11290723, 11292175, 11297364, 11297377, 11309623, 11309934, 11337174, 11337664, 11337680, 11337692)
    AND [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'
    AND Target.[System.AreaPath] UNDER 'OS\Core\WebPlat'
    AND Target.[System.WorkItemType] IN ('Customer Promise', 'Scenario', 'Measure', 'Deliverable', 'Task')
mode(Recursive)
";

            var start = Clock.GetTimestamp();
            RestResult.WorkItemLinks = RestResult.WorkItemStore.QueryLinks(WIQL).ToList();
            RestResult.WorkItems = RestResult
                .WorkItemStore.Query(new HashSet<int>(RestResult.WorkItemLinks.SelectMany(dl => new[] { dl.TargetId, dl.SourceId }).Where(i => i != 0)))
                .ToList();
            var stop = Clock.GetTimestamp();
            Debug.Print("REST: {0}", Clock.GetTimeSpan(start, stop));

            start = Clock.GetTimestamp();
            SoapResult.WorkItemLinks = SoapResult.WorkItemStore.QueryLinks(WIQL).ToList();
            SoapResult.WorkItems = SoapResult
                .WorkItemStore.Query(new HashSet<int>(SoapResult.WorkItemLinks.SelectMany(dl => new[] { dl.TargetId, dl.SourceId }).Where(i => i != 0)))
                .ToList();
            stop = Clock.GetTimestamp();
            Debug.Print("SOAP: {0}", Clock.GetTimeSpan(start, stop));
        }

        public override void Cleanup()
        {
            SoapResult?.Dispose();
            RestResult?.Dispose();
        }

        protected class Result : IDisposable
        {
            public IEnumerable<IWorkItem> WorkItems { get; set; }

            public IEnumerable<IWorkItemLinkInfo> WorkItemLinks { get; set; }

            public IWorkItemStore WorkItemStore { get; set; }

            public void Dispose()
            {
                WorkItemStore?.Dispose();
            }
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Link_Count_Equal()
        {
            RestResult.WorkItemLinks.Count().ShouldEqual(SoapResult.WorkItemLinks.Count(), "WorkItemLinks.Count");



        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void WorkItem_Count_Equal()
        {
            RestResult.WorkItems.Count().ShouldEqual(SoapResult.WorkItems.Count(), "WorkItems.Count");
        }

        //[TestMethod]
        //[TestCategory("localOnly")]
        //public void Links_Equal()
        //{
        //    RestResult.WorkItemLinks.ShouldContainOnly(SoapResult.WorkItemLinks);
        //}

        //[TestMethod]
        //[TestCategory("localOnly")]
        //public void WorkItems_Equal()
        //{
        //    RestResult.WorkItems.ShouldContainOnly(SoapResult.WorkItems);
        //}
    }

    [TestClass]
    public class LinkTypeTests : ContextSpecification
    {
        private IWorkItemStore _soap;

        private IEnumerable<IWorkItemLinkType> _soapResult;
        private IWorkItemStore _rest;
        private IEnumerable<IWorkItemLinkType> _restResult;

        public override void Given()
        {
            var credentials = Credentials.CredentialsFactory.CreateCredentials((string)null);
            var fac = WorkItemStoreFactory.GetInstance();
            var uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

            _soap = fac.Create(uri, credentials, ClientType.Soap);
            _rest = fac.Create(uri, credentials, ClientType.Rest);
        }

        public override void When()
        {
            _soapResult = _soap.WorkItemLinkTypes.ToList();
            _restResult = _rest.WorkItemLinkTypes.ToList();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Equal()
        {
            _restResult.ShouldContainOnly(_soapResult);
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
            RestResult.WorkItemLinks.ShouldContainOnly(SoapResult.WorkItemLinks, WorkItemLinkInfoEqualityComparer.Instance);
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
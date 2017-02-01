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
    public class WiqlHierarchyQueryTests : ContextSpecification
    {
        protected Result RestResult { get; set; }

        protected Result SoapResult { get; set; }

        public override void Given()
        {
            var credentials = Credentials.CredentialsFactory.CreateCredentials();
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
        public void SOAP_Links_returned()
        {
            SoapResult.WorkItemLinks.ShouldNotBeNull();
        }

        [TestMethod]
        public void REST_Links_returned()
        {
            RestResult.WorkItemLinks.ShouldNotBeNull();
        }

        [TestMethod]
        public void Same_number_of_links_returned()
        {
            RestResult.WorkItemLinks.Count().ShouldEqual(SoapResult.WorkItemLinks.Count());
        }

        [TestMethod]
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
        public void AreaPath_is_equal()
        {
            RestResult.WorkItem.AreaPath.ShouldEqual(SoapResult.WorkItem.AreaPath);
        }

        [TestMethod]
        public void AssignedTo_is_equal()
        {
            RestResult.WorkItem.AssignedTo.ShouldEqual(SoapResult.WorkItem.AssignedTo);
        }

        [TestMethod]
        public void ChangedBy_is_equal()
        {
            RestResult.WorkItem.ChangedBy.ShouldEqual(SoapResult.WorkItem.ChangedBy);
        }

        [TestMethod]
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
                    "System.State", "System.Title", "System.WorkItemType",
                };

        [TestMethod]
        public void CoreFields_are_equal()
        {
            

            foreach (var field in CoreFields)
            {
                RestResult.WorkItem[field].ShouldEqual(SoapResult.WorkItem[field]);
            }
        }

        [TestMethod]
        public void CreatedBy_is_equal()
        {
            RestResult.WorkItem.CreatedBy.ShouldEqual(SoapResult.WorkItem.CreatedBy);
        }

        [TestMethod]
        public void Rev_is_equal()
        {
            RestResult.WorkItem.Rev.ShouldEqual(SoapResult.WorkItem.Rev);
        }

        [TestMethod]
        public void CreatedDate_is_equal()
        {
            RestResult.WorkItem.CreatedDate.ShouldEqual(SoapResult.WorkItem.CreatedDate);
        }

        [TestMethod]
        public void Description_is_equal()
        {
            RestResult.WorkItem.Description.ShouldEqual(SoapResult.WorkItem.Description);
        }

        public override void Given()
        {
            var credentials = Credentials.CredentialsFactory.CreateCredentials();
            var fac = WorkItemStoreFactory.GetInstance();
            var uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

            SoapResult = new Result() { WorkItemStore = fac.Create(uri, credentials, ClientType.Soap) };
            RestResult = new Result() { WorkItemStore = fac.Create(uri, credentials, ClientType.Rest) };
        }

        [TestMethod]
        public void History_is_equal()
        {
            RestResult.WorkItem.History.ShouldEqual(SoapResult.WorkItem.History);
        }

        [TestMethod]
        public void Id_is_equal()
        {
            RestResult.WorkItem.Id.ShouldEqual(SoapResult.WorkItem.Id);
        }

        [TestMethod]
        public void IterationPath_is_equal()
        {
            RestResult.WorkItem.IterationPath.ShouldEqual(SoapResult.WorkItem.IterationPath);
        }

        [TestMethod]
        public void REST_WorkItem_is_returned()
        {
            RestResult.WorkItem.ShouldNotBeNull();
        }

        [TestMethod]
        public void SOAP_WorkItem_is_returned()
        {
            SoapResult.WorkItem.ShouldNotBeNull();
        }

        [TestMethod]
        public void State_is_equal()
        {
            RestResult.WorkItem.State.ShouldEqual(SoapResult.WorkItem.State);
        }

        [TestMethod]
        public void Tags_is_equal()
        {
            RestResult.WorkItem.Tags.ShouldEqual(SoapResult.WorkItem.Tags);
        }

        [TestMethod]
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
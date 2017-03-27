using System;
using System.Collections.Generic;

using Microsoft.Qwiq.Tests.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Core.Tests
{
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
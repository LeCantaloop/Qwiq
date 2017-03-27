using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Core.Tests
{
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
            var start = Clock.GetTimestamp();
            RestResult.WorkItemLinks = RestResult.WorkItemStore.QueryLinks(WIQL).ToList();
            var stop = Clock.GetTimestamp();
            Debug.Print("REST: {0}", Clock.GetTimeSpan(start, stop));

            start = Clock.GetTimestamp();
            SoapResult.WorkItemLinks = SoapResult.WorkItemStore.QueryLinks(WIQL).ToList();
            stop = Clock.GetTimestamp();
            Debug.Print("SOAP: {0}", Clock.GetTimeSpan(start, stop));
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
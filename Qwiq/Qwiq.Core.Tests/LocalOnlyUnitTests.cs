using System;
using System.Linq;
using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq.Credentials;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.IE.Qwiq.Core.Tests
{
    public class LocalOnlyTests : ContextSpecification
    {
        private const string VisualStudioResourceString = "499b84ac-1321-427f-aa17-267ca6975798";
        private const string TFSClientId = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";
        private const string Authority = "https://login.windows.net/common/";
        protected IWorkItemStore WorkItemStore { get; set; }

        public override void Given()
        {
            var credentials = CredentialsFactory.CreateAadCredentials(VisualStudioResourceString, TFSClientId, Authority);
            var connectionUri = new Uri("https://microsoft.visualstudio.com/DefaultCollection");
            WorkItemStore = WorkItemStoreFactory.GetInstance().Create(connectionUri, credentials);
        }
    }

    [TestClass]
    public class given_a_workitemstore_when_a_link_is_removed : LocalOnlyTests
    {
        private bool Result { get; set; }
        private IWorkItem TargetWorkItem { get; set; }
        private ILink TargetLink { get; set; }

        public override void Given()
        {
            base.Given();
            var end =
                WorkItemStore.WorkItemLinkTypes.Single(linkType => linkType.ReferenceName == "System.LinkTypes.Hierarchy").ForwardEnd;
            IWorkItem wi = WorkItemStore.Query("SELECT * FROM WorkItems WHERE [System.ID] IN (1542232)").Single();
            TargetWorkItem = WorkItemStore.Query("SELECT * FROM WorkItems WHERE [System.ID] IN (1523767)").Single();
            TargetLink = TargetWorkItem.CreateRelatedLink(end, wi);
        }

        public override void When()
        {
            Result = TargetWorkItem.Links.Remove(TargetLink);
        }

        [TestCategory("localOnly")]
        [TestMethod]
        public void the_result_of_the_removal_is_true()
        {
            Result.ShouldBeTrue();
        }

        [TestCategory("localOnly")]
        [TestMethod]
        public void the_link_is_actually_removed()
        {
            TargetWorkItem.Links.Contains(TargetLink).ShouldBeFalse();
        }
    }
}

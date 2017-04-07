using System;
using System.Collections.Generic;

using Microsoft.Qwiq.Core.Tests;
using Microsoft.Qwiq.Credentials;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Integration.Tests
{
    public abstract class WorkItemStoreComparisonContextSpecification : TimedContextSpecification
    {
        protected IWorkItemStore Rest => RestResult.WorkItemStore;

        protected IWorkItemStore Soap => SoapResult.WorkItemStore;

        protected Result RestResult { get; private set; }
        protected Result SoapResult { get; private set; }

        public override void Given()
        {
            var uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

            var options = new AuthenticationOptions(uri, AuthenticationType.Windows) { CreateCredentials = CreateCredentials };

            SoapResult = new Result()
                             {
                                 WorkItemStore = TimedAction(
                                     () => Microsoft.Qwiq.Soap.WorkItemStoreFactory.Instance.Create(options),
                                     "SOAP",
                                     "Create WIS")
                             };

            options.ClientType = ClientType.Rest;

            RestResult = new Result()
                             {
                                 WorkItemStore = TimedAction(
                                     () => Microsoft.Qwiq.Soap.WorkItemStoreFactory.Instance.Create(options),
                                     "REST",
                                     "Create WIS")
                             };

        }

        private static IEnumerable<TfsCredentials> CreateCredentials(AuthenticationType t)
        {
            // User did not specify a username or a password, so use the process identity
            yield return new VssClientCredentials(new WindowsCredential(false)) { Storage = new VssClientCredentialStorage(), PromptType = CredentialPromptType.DoNotPrompt };

            // Use the Windows identity of the logged on user
            yield return new VssClientCredentials(true) { Storage = new VssClientCredentialStorage(), PromptType = CredentialPromptType.PromptIfNeeded };
        }

        public override void Cleanup()
        {
            Rest?.Dispose();
            Soap?.Dispose();

            base.Cleanup();
        }

        
    }

    public abstract class SingleWorkItemComparisonContextSpecification : WorkItemStoreComparisonContextSpecification
    {
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void WorkItem_is_equal()
        {
            RestResult.WorkItem.ShouldEqual(SoapResult.WorkItem);
            RestResult.WorkItem.GetHashCode().ShouldEqual(SoapResult.WorkItem.GetHashCode());
        }
    }
}

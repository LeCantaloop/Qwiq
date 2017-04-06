using System;
using System.Collections.Generic;

using Microsoft.Qwiq.Core.Tests;
using Microsoft.Qwiq.Credentials;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Integration.Tests
{
    public abstract class WorkItemStoreComparisonContextSpecification : TimedContextSpecification
    {
        protected IWorkItemStore Rest { get; private set; }

        protected IWorkItemStore Soap { get; private set; }

        public override void Given()
        {
            var uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

            var options = new AuthenticationOptions(uri, AuthenticationType.Windows) { CreateCredentials = CreateCredentials };

            Soap = TimedAction(
                () => Microsoft.Qwiq.Soap.WorkItemStoreFactory.Instance.Create(options),
                "SOAP",
                "Create WIS");

            options.ClientType = ClientType.Rest;

            Rest = TimedAction(
                () => Microsoft.Qwiq.Soap.WorkItemStoreFactory.Instance.Create(options),
                "REST",
                "Create WIS");

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
}

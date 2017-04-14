using System;
using System.Collections.Generic;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Soap;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Integration.Tests
{
    public static class IntegrationSettings
    {
        public static readonly Func<IWorkItemStore> CreateRestStore = () =>
                                                                          {
                                                                              var options = IntegrationOptions();
                                                                              options.ClientType = ClientType.Rest;
                                                                              return WorkItemStoreFactory.Instance.Create(options);
                                                                          };

        public static readonly Func<IWorkItemStore> CreateSoapStore = () =>
                                                                          {
                                                                              var options = IntegrationOptions();
                                                                              return WorkItemStoreFactory.Instance.Create(options);
                                                                          };

        private static readonly Func<AuthenticationOptions> IntegrationOptions = Options;

        private static IEnumerable<TfsCredentials> CreateCredentials(AuthenticationType t)
        {
            // User did not specify a username or a password, so use the process identity
            yield return new VssClientCredentials(new WindowsCredential(false))
                             {
                                 Storage = new VssClientCredentialStorage(),
                                 PromptType = CredentialPromptType.DoNotPrompt
                             };

            // Use the Windows identity of the logged on user
            yield return new VssClientCredentials(true)
                             {
                                 Storage = new VssClientCredentialStorage(),
                                 PromptType = CredentialPromptType.PromptIfNeeded
                             };
        }

        private static AuthenticationOptions Options()
        {
            var uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");
            return new AuthenticationOptions(uri, AuthenticationType.Windows) { CreateCredentials = CreateCredentials };
        }
    }
}
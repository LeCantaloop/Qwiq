using Qwiq.Credentials;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Qwiq
{
    /// <exclude />
    public static class IntegrationSettings
    {
        /// <exclude />
        public static Func<AuthenticationTypes, IEnumerable<VssCredentials>> Credentials = UnitTestCredentialsFactory;

        /// <exclude />
        public static string[] Domains = { "microsoft.com" };

        /// <exclude />
        public static Guid ProjectGuid = Guid.Parse("8d47e068-03c8-4cdc-aa9b-fc6929290322");

        /// <exclude />
        public static string TenantId = "72F988BF-86F1-41AF-91AB-2D7CD011DB47";

        private static readonly Uri Uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

        /// <exclude />
        public static AuthenticationOptions AuthenticationOptions { get; } = new AuthenticationOptions(Uri, AuthenticationTypes.Windows, Credentials);

        /// <exclude />
        public static Func<IWorkItemStore> CreateRestStore { get; } = () =>
                                                                          {
                                                                              var options = AuthenticationOptions;
                                                                              var wis = Client.Rest.WorkItemStoreFactory.Default.Create(options);
                                                                              Configure(wis);
                                                                              return wis;
                                                                          };

        private static void Configure(IWorkItemStore wis)
        {
            wis.Configuration.PageSize = 200;
            wis.Configuration.LazyLoadingEnabled = true;
            wis.Configuration.ProxyCreationEnabled = true;
        }

        /// <exclude />
        public static Func<IWorkItemStore> CreateSoapStore { get; } = () =>
                                                                          {
                                                                              var options = AuthenticationOptions;
                                                                              var wis = Client.Soap.WorkItemStoreFactory.Default.Create(options);
                                                                              Configure(wis);
                                                                              return wis;
                                                                          };

        /// <exclude />
        public static bool IsContiniousIntegrationEnvironment { get; } =
            Comparer.OrdinalIgnoreCase.Equals("True", Environment.GetEnvironmentVariable("CI"))
            || Comparer.OrdinalIgnoreCase.Equals("True", Environment.GetEnvironmentVariable("APPVEYOR"));

        private static IEnumerable<VssCredentials> UnitTestCredentialsFactory(AuthenticationTypes types)
        {
            if (types.HasFlag(AuthenticationTypes.Windows))
            {
                // User did not specify a username or a password, so use the process identity
                yield return new VssClientCredentials(new WindowsCredential(false)) { Storage = new VssClientCredentialStorage(), PromptType = CredentialPromptType.DoNotPrompt };

                if (IsContiniousIntegrationEnvironment) yield break;

                // Use the Windows identity of the logged on user
                yield return new VssClientCredentials(true) { Storage = new VssClientCredentialStorage(), PromptType = CredentialPromptType.PromptIfNeeded };
            }
        }
    }
}
using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Rest.Proxies;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Rest
{
    public class WorkItemStoreFactory : IWorkItemStoreFactory
    {
        public static readonly IWorkItemStoreFactory Instance = Nested.Instance;

        private WorkItemStoreFactory()
        {
        }

        public IWorkItemStore Create(AuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            var credentials = options.Credentials;

            foreach (var credential in credentials)
                try
                {
                    var tfsNative = ConnectToTfsCollection(options.Uri, credential.Credentials);
                    var tfsProxy =
                        ExceptionHandlingDynamicProxyFactory.Create<IInternalTfsTeamProjectCollection>(
                            new VssConnectionAdapter(tfsNative));

                    options.Notifications.AuthenticationSuccess(
                        new AuthenticationSuccessNotification(credential, tfsProxy));

                    IWorkItemStore wis;
                    switch (options.ClientType)
                    {
                        case ClientType.Rest:
                            wis = CreateRestWorkItemStore(tfsProxy);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(options.ClientType));
                    }

                    return ExceptionHandlingDynamicProxyFactory.Create(wis);
                }
                catch (Exception e)
                {
                    options.Notifications.AuthenticationFailed(new AuthenticationFailedNotification(credential, e));
                }

            var nocreds = new AccessDeniedException("Invalid credentials");
            options.Notifications.AuthenticationFailed(new AuthenticationFailedNotification(null, nocreds));
            throw nocreds;
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.",
            false)]
        public IWorkItemStore Create(Uri endpoint, TfsCredentials credentials, ClientType type = ClientType.Default)
        {
            return Create(endpoint, new[] { credentials }, type);
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.",
            false)]
        public IWorkItemStore Create(
            Uri endpoint,
            IEnumerable<TfsCredentials> credentials,
            ClientType type = ClientType.Default)
        {
            var options =
                new AuthenticationOptions(endpoint, AuthenticationType.Windows, type)
                    {
                        CreateCredentials =
                            t => credentials
                    };

            return Create(options);
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See property Instance instead.",
            false)]
        public static IWorkItemStoreFactory GetInstance()
        {
            return Instance;
        }

        private static VssConnection ConnectToTfsCollection(Uri endpoint, VssCredentials credentials)
        {
            var tfsServer = new VssConnection(endpoint, credentials);
            tfsServer.ConnectAsync(VssConnectMode.Automatic).GetAwaiter().GetResult();
            if (!tfsServer.HasAuthenticated) throw new InvalidOperationException("Could not connect.");
            return tfsServer;
        }

        private static IWorkItemStore CreateRestWorkItemStore(IInternalTfsTeamProjectCollection tfs)
        {
            return new WorkItemStoreProxy(() => tfs, QueryFactory.GetInstance);
        }

        private class Nested
        {
            internal static readonly WorkItemStoreFactory Instance = new WorkItemStoreFactory();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}
using System;
using System.Collections.Generic;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Soap
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
            switch (options.ClientType)
            {
                case ClientType.Soap:
                    var credentials = options.Credentials;
                    foreach (var credential in credentials)
                        try
                        {
                            var tfsNative = ConnectToTfsCollection(options.Uri, credential.Credentials);
                            var tfsProxy =
                                ExceptionHandlingDynamicProxyFactory.Create<IInternalTfsTeamProjectCollection>(
                                    new TfsTeamProjectCollection(tfsNative));

                            options.Notifications.AuthenticationSuccess(
                                new AuthenticationSuccessNotification(credential, tfsProxy));
                            return CreateSoapWorkItemStore(tfsProxy);
                        }
                        catch (TeamFoundationServerUnauthorizedException e)
                        {
                            options.Notifications.AuthenticationFailed(
                                new AuthenticationFailedNotification(credential, e));
                        }
                    var nocreds = new AccessDeniedException("Invalid credentials");
                    options.Notifications.AuthenticationFailed(new AuthenticationFailedNotification(null, nocreds));
                    throw nocreds;
                case ClientType.Rest:
                    return Rest.WorkItemStoreFactory.Instance.Create(options);

                default:
                    throw new ArgumentOutOfRangeException(nameof(options.ClientType));
            }
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

        private static TeamFoundation.Client.TfsTeamProjectCollection ConnectToTfsCollection(Uri endpoint, VssCredentials credentials)
        {
            var tfsServer = new TeamFoundation.Client.TfsTeamProjectCollection(endpoint, credentials);
            tfsServer.EnsureAuthenticated();
            return tfsServer;
        }

        private static IWorkItemStore CreateSoapWorkItemStore(IInternalTfsTeamProjectCollection tfs)
        {
            return new WorkItemStore(() => tfs, QueryFactory.GetInstance);
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class Nested
        // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly WorkItemStoreFactory Instance = new WorkItemStoreFactory();
            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}
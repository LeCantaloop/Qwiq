using System;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Soap
{
    public class WorkItemStoreFactory : IWorkItemStoreFactory
    {
        public static readonly IWorkItemStoreFactory Instance = Nested.Instance;

        private WorkItemStoreFactory()
        {
        }

        [Obsolete("This method is deprecated and will be removed in a future release. See property Instance instead.", false)]
        public static IWorkItemStoreFactory GetInstance()
        {
            return Instance;
        }

        public IWorkItemStore Create(IAuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var credentials = options.Credentials;
            foreach (var credential in credentials)
            {
                try
                {
                    var tfsNative = ConnectToTfsCollection(options.Uri, credential);
                    var tfsProxy =
                            ExceptionHandlingDynamicProxyFactory
                                    .Create<IInternalTfsTeamProjectCollection>(new TfsTeamProjectCollection(tfsNative));

                    options.Notifications.AuthenticationSuccess(new AuthenticationSuccessNotification(credential, tfsProxy));
                    return CreateSoapWorkItemStore(tfsProxy);
                }
                catch (TeamFoundationServerUnauthorizedException e)
                {
                    options.Notifications.AuthenticationFailed(new AuthenticationFailedNotification(credential, e));
                }
            }
            var nocreds = new AccessDeniedException("Invalid credentials");
            options.Notifications.AuthenticationFailed(new AuthenticationFailedNotification(null, nocreds));
            throw nocreds;
        }

        private static TeamFoundation.Client.TfsTeamProjectCollection ConnectToTfsCollection(Uri endpoint, VssCredentials credentials)
        {
            var tfsServer = new TeamFoundation.Client.TfsTeamProjectCollection(endpoint, credentials);
            tfsServer.EnsureAuthenticated();
            return tfsServer;
        }

        private static IWorkItemStore CreateSoapWorkItemStore(IInternalTfsTeamProjectCollection tfs)
        {
            return new WorkItemStore(() => tfs, store => new QueryFactory(store));
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
using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Qwiq.Rest
{
    public class WorkItemStoreFactory : IWorkItemStoreFactory
    {
        public static readonly IWorkItemStoreFactory Instance = Nested.Instance;

        private WorkItemStoreFactory()
        {
        }

        public IWorkItemStore Create(IAuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            var credentials = options.Credentials;

            foreach (var credential in credentials)
                try
                {
                    var tfsNative = ConnectToTfsCollection(options.Uri, credential);
                    var tfsProxy =
                        ExceptionHandlingDynamicProxyFactory.Create<IInternalTfsTeamProjectCollection>(
                            new VssConnectionAdapter(tfsNative));

                    options.Notifications.AuthenticationSuccess(new AuthenticationSuccessNotification(credential, tfsProxy));

                    var wis = CreateRestWorkItemStore(tfsProxy);
                    

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

        private static VssConnection ConnectToTfsCollection(Uri endpoint, VssCredentials credentials)
        {
            var tfsServer = new VssConnection(endpoint, credentials);
            tfsServer.ConnectAsync(VssConnectMode.Automatic).GetAwaiter().GetResult();
            if (!tfsServer.HasAuthenticated) throw new InvalidOperationException("Could not connect.");
            return tfsServer;
        }

        private static IWorkItemStore CreateRestWorkItemStore(IInternalTfsTeamProjectCollection tfs)
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
using System;

using Microsoft.Qwiq.Credentials;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Client.Soap
{
    public class TfsConnectionFactory : Qwiq.TfsConnectionFactory
    {
        public static readonly ITfsConnectionFactory Default = Nested.Instance;

        private TfsConnectionFactory()
        {
        }

        /// <inheritdoc />
        public override ITeamProjectCollection Create(AuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            var credentials = options.Credentials;

            foreach (var credential in credentials)
            {
                try
                {
                    var tfsNative = ConnectToTfsCollection(options.Uri, credential);
                    var tfsProxy = tfsNative.AsProxy();

                    options.Notifications.AuthenticationSuccess(new AuthenticationSuccessNotification(credential, tfsProxy));

                    return tfsProxy;
                }
                catch (Exception e)
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

        // ReSharper disable ClassNeverInstantiated.Local
        private class Nested
                // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly TfsConnectionFactory Instance = new TfsConnectionFactory();
            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}
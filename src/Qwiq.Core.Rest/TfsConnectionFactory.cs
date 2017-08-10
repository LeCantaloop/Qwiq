using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Qwiq.Client.Rest
{
    public class TfsConnectionFactory : TfsConnectionFactory<ITeamProjectCollection>
    {
        public static readonly ITfsConnectionFactory Default = Nested.Instance;

        private TfsConnectionFactory()
        {
        }

        protected override ITeamProjectCollection ConnectToTfsCollection(Uri endpoint, VssCredentials credentials)
        {
            var tfsServer = new VssConnection(endpoint, credentials);

            tfsServer.Settings.BypassProxyOnLocal = true;
            tfsServer.Settings.CompressionEnabled = true;

            tfsServer.ConnectAsync(VssConnectMode.Automatic).GetAwaiter().GetResult();
            if (!tfsServer.HasAuthenticated) throw new InvalidOperationException("Could not connect.");
            return tfsServer.AsProxy();
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
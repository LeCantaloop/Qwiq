using System;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Client.Soap
{
    public class TfsConnectionFactory : TfsConnectionFactory<ITeamProjectCollection>
    {
        public static readonly ITfsConnectionFactory Default = Nested.Instance;

        private TfsConnectionFactory()
        {
        }

        /// <inheritdoc />
        protected override ITeamProjectCollection ConnectToTfsCollection(Uri endpoint, VssCredentials credentials)
        {
            var tfsServer = new TeamFoundation.Client.TfsTeamProjectCollection(endpoint, credentials);
            tfsServer.EnsureAuthenticated();
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
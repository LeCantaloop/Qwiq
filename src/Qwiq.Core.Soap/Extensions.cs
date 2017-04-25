using Microsoft.Qwiq.Exceptions;

namespace Microsoft.Qwiq.Soap
{
    internal static class Extensions
    {
        internal static ITeamFoundationIdentity AsProxy(this TeamFoundation.Framework.Client.TeamFoundationIdentity identity)
        {
            return identity == null
                ? null
                : ExceptionHandlingDynamicProxyFactory.Create<ITeamFoundationIdentity>(new TeamFoundationIdentity(identity));
        }

        internal static IInternalTeamProjectCollection AsProxy(this TeamFoundation.Client.TfsTeamProjectCollection tfsNative)
        {
            return tfsNative == null
                       ? null
                       : ExceptionHandlingDynamicProxyFactory.Create<IInternalTeamProjectCollection>(new TfsTeamProjectCollection(tfsNative));
        }
    }
}

using Qwiq.Exceptions;

namespace Qwiq.Client.Soap
{
    internal static class Extensions
    {
        internal static ITeamFoundationIdentity AsProxy(this Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity identity)
        {
            return identity == null
                ? null
                : ExceptionHandlingDynamicProxyFactory.Create<ITeamFoundationIdentity>(new TeamFoundationIdentity(identity));
        }

        internal static IInternalTeamProjectCollection AsProxy(this Microsoft.TeamFoundation.Client.TfsTeamProjectCollection tfsNative)
        {
            return tfsNative == null
                       ? null
                       : ExceptionHandlingDynamicProxyFactory.Create<IInternalTeamProjectCollection>(new TfsTeamProjectCollection(tfsNative));
        }
    }
}

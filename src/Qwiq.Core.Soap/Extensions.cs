using Qwiq.Exceptions;

namespace Qwiq.Client.Soap
{
    internal static class Extensions
    {
        internal static ITeamFoundationIdentity AsProxy(this Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity identity)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<ITeamFoundationIdentity>(new TeamFoundationIdentity(identity!));
        }

        internal static IInternalTeamProjectCollection AsProxy(this Microsoft.TeamFoundation.Client.TfsTeamProjectCollection tfsNative)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IInternalTeamProjectCollection>(new TfsTeamProjectCollection(tfsNative!));
        }
    }
}

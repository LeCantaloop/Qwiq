using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.Qwiq.Identity.Soap
{
    public static class Extensions
    {
        internal static IIdentityDescriptor AsProxy(this TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(new Microsoft.Qwiq.Soap.IdentityDescriptor(descriptor));
        }

        internal static IIdentityManagementService AsProxy(this IIdentityManagementService2 ims)
        {
            return ims == null
                ? null
                : ExceptionHandlingDynamicProxyFactory.Create<IIdentityManagementService>(new IdentityManagementService(ims));
        }

        public static IIdentityManagementService GetIdentityManagementService(this ITeamProjectCollection tpc)
        {
            return ((Qwiq.Soap.IInternalTeamProjectCollection)tpc).GetService<IIdentityManagementService2>().AsProxy();
        }

        public static IIdentityManagementService GetIdentityManagementService(this IWorkItemStore wis)
        {
            return wis.TeamProjectCollection.GetIdentityManagementService();
        }
    }
}

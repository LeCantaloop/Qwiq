using System;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

using Microsoft.Qwiq.Client.Soap;
using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.Qwiq.Identity.Soap
{
    public static class Extensions
    {
        [NotNull]
        [JetBrains.Annotations.Pure]
        public static IIdentityManagementService GetIdentityManagementService([NotNull] this ITeamProjectCollection tpc)
        {
            Contract.Requires(tpc != null);

            if (tpc == null) throw new ArgumentNullException(nameof(tpc));
            return ((IInternalTeamProjectCollection)tpc).GetService<IIdentityManagementService2>().AsProxy();
        }

        [NotNull]
        [JetBrains.Annotations.Pure]
        public static IIdentityManagementService GetIdentityManagementService([NotNull] this IWorkItemStore wis)
        {
            if (wis == null) throw new ArgumentNullException(nameof(wis));
            return wis.TeamProjectCollection.GetIdentityManagementService();
        }

        [NotNull]
        [JetBrains.Annotations.Pure]
        internal static IIdentityDescriptor AsProxy([NotNull] this TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            return ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(new Client.Soap.IdentityDescriptor(descriptor));
        }

        [JetBrains.Annotations.Pure]
        [CanBeNull]
        internal static IIdentityManagementService AsProxy([CanBeNull] this IIdentityManagementService2 ims)
        {
            return ims == null
                       ? null
                       : ExceptionHandlingDynamicProxyFactory.Create<IIdentityManagementService>(new IdentityManagementService(ims));
        }
    }
}
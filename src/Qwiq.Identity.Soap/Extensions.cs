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
        /// <summary>
        /// Gets the identity management service from an instance of <see cref="ITeamProjectCollection"/>.
        /// </summary>
        /// <param name="teamProjectCollection">An instance of <see cref="ITeamProjectCollection"/></param>
        /// <returns><see cref="IIdentityManagementService" />.</returns>
        /// <exception cref="ArgumentNullException">teamProjectCollection</exception>
        [NotNull]
        [JetBrains.Annotations.Pure]
        [PublicAPI]
        public static IIdentityManagementService GetIdentityManagementService([NotNull] this ITeamProjectCollection teamProjectCollection)
        {
            Contract.Requires(teamProjectCollection != null);

            if (teamProjectCollection == null) throw new ArgumentNullException(nameof(teamProjectCollection));
            return ((IInternalTeamProjectCollection)teamProjectCollection).GetService<IIdentityManagementService2>().AsProxy();
        }

        /// <summary>
        /// Gets the identity management service from an instance of <see cref="IWorkItemStore"/>.
        /// </summary>
        /// <param name="workItemStore">An instance of <see cref="IWorkItemStore"/>.</param>
        /// <returns><see cref="IIdentityManagementService" />.</returns>
        /// <exception cref="ArgumentNullException">workItemStore</exception>
        [NotNull]
        [JetBrains.Annotations.Pure]
        [PublicAPI]
        public static IIdentityManagementService GetIdentityManagementService([NotNull] this IWorkItemStore workItemStore)
        {
            if (workItemStore == null) throw new ArgumentNullException(nameof(workItemStore));
            return workItemStore.TeamProjectCollection.GetIdentityManagementService();
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
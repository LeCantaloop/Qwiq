using System;
using System.Diagnostics.Contracts;


using Qwiq.Client.Soap;
using Qwiq.Exceptions;
using Microsoft.TeamFoundation.Framework.Client;

namespace Qwiq.Identity.Soap
{
    public static class Extensions
    {
        /// <summary>
        /// Gets the identity management service from an instance of <see cref="ITeamProjectCollection"/>.
        /// </summary>
        /// <param name="teamProjectCollection">An instance of <see cref="ITeamProjectCollection"/></param>
        /// <returns><see cref="IIdentityManagementService" />.</returns>
        /// <exception cref="ArgumentNullException">teamProjectCollection</exception>
        public static IIdentityManagementService GetIdentityManagementService(this ITeamProjectCollection teamProjectCollection)
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
        public static IIdentityManagementService GetIdentityManagementService(this IWorkItemStore workItemStore)
        {
            if (workItemStore == null) throw new ArgumentNullException(nameof(workItemStore));
            return workItemStore.TeamProjectCollection.GetIdentityManagementService();
        }
        internal static IIdentityDescriptor AsProxy(this Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            return ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(new Client.Soap.IdentityDescriptor(descriptor));
        }

        internal static IIdentityManagementService AsProxy(this IIdentityManagementService2 ims)
        {
            return ims == null
                       ? null
                       : ExceptionHandlingDynamicProxyFactory.Create<IIdentityManagementService>(new IdentityManagementService(ims));
        }
    }
}
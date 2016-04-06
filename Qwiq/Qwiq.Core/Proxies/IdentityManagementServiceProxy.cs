using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.Qwiq.Exceptions;
using Tfs = Microsoft.TeamFoundation.Framework;

namespace Microsoft.IE.Qwiq.Proxies
{
    public class IdentityManagementServiceProxy : IIdentityManagementService
    {
        private readonly Tfs.Client.IIdentityManagementService2 _identityManagementService2;

        internal IdentityManagementServiceProxy(Tfs.Client.IIdentityManagementService2 identityManagementService2)
        {
            _identityManagementService2 = identityManagementService2;
        }

        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(IEnumerable<IIdentityDescriptor> descriptors)
        {
            var rawDescriptors = descriptors.Select(descriptor =>
                new Tfs.Client.IdentityDescriptor(descriptor.IdentityType, descriptor.Identifier)).ToArray();

            var identities = _identityManagementService2.ReadIdentities(rawDescriptors, Tfs.Common.MembershipQuery.None,
                Tfs.Common.ReadIdentityOptions.IncludeReadFromSource);

            return identities.Select(identity => identity == null ? null : ExceptionHandlingDynamicProxyFactory.Create<ITeamFoundationIdentity>(new TeamFoundationIdentityProxy(identity)));
        }

        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(IdentitySearchFactor searchFactor, string[] searchFactorValues)
        {
            var factor = (Tfs.Common.IdentitySearchFactor) searchFactor;
            var identities = _identityManagementService2.ReadIdentities(factor, searchFactorValues,
                Tfs.Common.MembershipQuery.None, Tfs.Common.ReadIdentityOptions.IncludeReadFromSource)[0];

            return identities.Select(identity => identity == null ? null : ExceptionHandlingDynamicProxyFactory.Create<ITeamFoundationIdentity>(new TeamFoundationIdentityProxy(identity)));
        }

        public IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(new IdentityDescriptorProxy(new Tfs.Client.IdentityDescriptor(identityType, identifier)));
        }
    }
}
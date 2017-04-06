using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.Framework;

namespace Microsoft.Qwiq.Soap
{
    internal class IdentityManagementService : IIdentityManagementService
    {
        private readonly Tfs.Client.IIdentityManagementService2 _identityManagementService2;

        internal IdentityManagementService(Tfs.Client.IIdentityManagementService2 identityManagementService2)
        {
            _identityManagementService2 = identityManagementService2 ?? throw new ArgumentNullException(nameof(identityManagementService2));
        }

        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(ICollection<IIdentityDescriptor> descriptors)
        {
            var rawDescriptors = descriptors.Select(descriptor =>
                new Tfs.Client.IdentityDescriptor(descriptor.IdentityType, descriptor.Identifier)).ToArray();

            var identities = _identityManagementService2.ReadIdentities(rawDescriptors, Tfs.Common.MembershipQuery.None,
                Tfs.Common.ReadIdentityOptions.IncludeReadFromSource);

            return identities.Select(identity => identity == null ? null : ExceptionHandlingDynamicProxyFactory.Create<ITeamFoundationIdentity>(new TeamFoundationIdentity(identity)));
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(IdentitySearchFactor searchFactor, ICollection<string> searchFactorValues)
        {
            var searchFactorArray = searchFactorValues.ToArray();
            var factor = (Tfs.Common.IdentitySearchFactor) searchFactor;
            var identities = _identityManagementService2.ReadIdentities(factor, searchFactorArray,
                Tfs.Common.MembershipQuery.None, Tfs.Common.ReadIdentityOptions.IncludeReadFromSource);

            if (searchFactorArray.Length != identities.Length)
            {
                throw new IndexOutOfRangeException("A call to IIdentityManagementService2.ReadIdentities resulted in a return set where there was not a one to one mapping between search terms and search results. This is unexpected behavior and execution cannot continue. Please check if the underlying service implementation has changed and update the consuming code as appropriate.");
            }

            for (var i = 0; i < searchFactorArray.Length; i++)
            {
                var proxiedIdentities = identities[i].Select(TryCreateProxy);
                yield return new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(searchFactorArray[i], proxiedIdentities);
            }
        }

        public IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(new IdentityDescriptor(new Tfs.Client.IdentityDescriptor(identityType, identifier)));
        }

        private ITeamFoundationIdentity TryCreateProxy(Tfs.Client.TeamFoundationIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }

            return
                ExceptionHandlingDynamicProxyFactory.Create<ITeamFoundationIdentity>(
                    new TeamFoundationIdentity(identity));
        }
    }
}

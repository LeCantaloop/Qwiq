using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Client.Soap;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace Microsoft.Qwiq.Identity.Soap
{
    internal class IdentityManagementService : IIdentityManagementService
    {
        private readonly IIdentityManagementService2 _identityManagementService2;

        internal IdentityManagementService(IIdentityManagementService2 identityManagementService2)
        {
            _identityManagementService2 = identityManagementService2 ?? throw new ArgumentNullException(nameof(identityManagementService2));
        }

        public IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier)
        {
            return new TeamFoundation.Framework.Client.IdentityDescriptor(identityType, identifier).AsProxy();
        }

        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(IEnumerable<IIdentityDescriptor> descriptors)
        {
            return ReadIdentities(descriptors, MembershipQuery.None);
        }

        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(
            IEnumerable<IIdentityDescriptor> descriptors,
            MembershipQuery queryMembership)
        {
            if (descriptors == null) throw new ArgumentNullException(nameof(descriptors));

            var rawDescriptors = descriptors.Select(
                    descriptor => new TeamFoundation.Framework.Client.IdentityDescriptor(
                        descriptor.IdentityType,
                        descriptor.Identifier))
                .ToArray();

            var identities =
                _identityManagementService2.ReadIdentities(
                    rawDescriptors,
                    (TeamFoundation.Framework.Common.MembershipQuery)queryMembership,
            ReadIdentityOptions.IncludeReadFromSource);

            // TODO: Use configuration options from IWorkItemStore to control proxy creation
            return identities.Select(identity => identity?.AsProxy());
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(
            IdentitySearchFactor searchFactor,
            IEnumerable<string> searchFactorValues)
        {
            return ReadIdentities(searchFactor, searchFactorValues, MembershipQuery.None);
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(
            IdentitySearchFactor searchFactor,
            IEnumerable<string> searchFactorValues,
            MembershipQuery queryMembership)
        {
            if (searchFactorValues == null) throw new ArgumentNullException(nameof(searchFactorValues));

            var searchFactorArray = searchFactorValues.ToArray();
            var factor = (TeamFoundation.Framework.Common.IdentitySearchFactor)searchFactor;
            var identities = _identityManagementService2.ReadIdentities(
                factor,
                searchFactorArray,
                (TeamFoundation.Framework.Common.MembershipQuery)queryMembership,
                ReadIdentityOptions.IncludeReadFromSource);

            if (searchFactorArray.Length != identities.Length)
                throw new IndexOutOfRangeException(
                    "A call to IIdentityManagementService2.ReadIdentities resulted in a return set where there was not a one to one mapping between search terms and search results. This is unexpected behavior and execution cannot continue. Please check if the underlying service implementation has changed and update the consuming code as appropriate.");

            for (var i = 0; i < searchFactorArray.Length; i++)
            {
                // TODO: Use configuration options from IWorkItemStore to control proxy creation
                var proxiedIdentities = identities[i].Select(id => id.AsProxy());
                yield return new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(searchFactorArray[i], proxiedIdentities);
            }
        }

        public ITeamFoundationIdentity ReadIdentity(
            IdentitySearchFactor searchFactor,
            string searchFactorValue)
        {
            return ReadIdentity(searchFactor, searchFactorValue, MembershipQuery.None);
        }

        public ITeamFoundationIdentity ReadIdentity(
            IdentitySearchFactor searchFactor,
            string searchFactorValue,
            MembershipQuery queryMembership)
        {
            if (string.IsNullOrEmpty(searchFactorValue)) throw new ArgumentException("Value cannot be null or empty.", nameof(searchFactorValue));

            // TODO: Use configuration options from IWorkItemStore to control proxy creation
            return _identityManagementService2.ReadIdentity(
                    (TeamFoundation.Framework.Common.IdentitySearchFactor)searchFactor,
                    searchFactorValue,
                    (TeamFoundation.Framework.Common.MembershipQuery)queryMembership,
                    ReadIdentityOptions.IncludeReadFromSource)
                .AsProxy();
        }
    }
}
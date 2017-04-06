using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace Microsoft.Qwiq.Core.Tests.Mocks
{
    public class MockIdentityManagementService2 : IIdentityManagementService2
    {
        private TeamFoundation.Framework.Client.TeamFoundationIdentity[] GetNullIdentities()
        {
            return new TeamFoundation.Framework.Client.TeamFoundationIdentity[]
            {
                null
            };
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] ReadIdentities(IdentityDescriptor[] descriptors, MembershipQuery queryMembership,
            ReadIdentityOptions readOptions)
        {
            return GetNullIdentities();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(IdentityDescriptor descriptor, MembershipQuery queryMembership,
            ReadIdentityOptions readOptions)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] ReadIdentities(Guid[] teamFoundationIds, MembershipQuery queryMembership)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[][] ReadIdentities(TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor, string[] searchFactorValues,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions)
        {
            return new []{ GetNullIdentities() };
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor, string searchFactorValue,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[][] ReadIdentities(IdentitySearchFactor searchFactor, string[] searchFactorValues,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(IdentitySearchFactor searchFactor, string searchFactorValue,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions)
        {
            throw new NotSupportedException();
        }

        public IdentityDescriptor CreateApplicationGroup(string scopeId, string groupName, string groupDescription)
        {
            throw new NotSupportedException();
        }

        public void DeleteApplicationGroup(IdentityDescriptor groupDescriptor)
        {
            throw new NotSupportedException();
        }

        public void AddMemberToApplicationGroup(IdentityDescriptor groupDescriptor, IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public void RemoveMemberFromApplicationGroup(IdentityDescriptor groupDescriptor, IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public bool IsMember(IdentityDescriptor groupDescriptor, IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public bool RefreshIdentity(IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public string GetScopeName(string scopeId)
        {
            throw new NotSupportedException();
        }

        public bool IsOwner(IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public bool IsOwnedWellKnownGroup(IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public string IdentityDomainScope
        {
            get { throw new NotSupportedException(); }
        }

        public void UpdateApplicationGroup(IdentityDescriptor groupDescriptor, GroupProperty groupProperty, string newValue)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] ListApplicationGroups(string scopeId, ReadIdentityOptions readOptions)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] GetMostRecentlyUsedUsers()
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] GetMostRecentlyUsedUsersEx(Guid teamId)
        {
            throw new NotSupportedException();
        }

        public void AddRecentUser(TeamFoundation.Framework.Client.TeamFoundationIdentity identity)
        {
            throw new NotSupportedException();
        }

        public void AddRecentUser(TeamFoundationIdentity identity)
        {
            throw new NotSupportedException();
        }

        public void AddRecentUser(Guid teamFoundationId)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(string generalSearchValue)
        {
            throw new NotSupportedException();
        }

        public FilteredIdentitiesList ReadFilteredIdentities(string expression, int suggestedPageSize, string lastSearchResult,
            bool lookForward, int queryMembership)
        {
            throw new NotSupportedException();
        }

        public void SetCustomDisplayName(string customDisplayName)
        {
            throw new NotSupportedException();
        }

        public void ClearCustomDisplayName()
        {
            throw new NotSupportedException();
        }

        public void UpdateExtendedProperties(TeamFoundation.Framework.Client.TeamFoundationIdentity identity)
        {
            throw new NotSupportedException();
        }

        public void UpdateExtendedProperties(TeamFoundationIdentity identity)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor, string searchFactorValue,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] ListApplicationGroups(string scopeId, ReadIdentityOptions readOptions,
            IEnumerable<string> propertyNameFilters, IdentityPropertyScope propertyScope)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(IdentitySearchFactor searchFactor, string searchFactorValue,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[][] ReadIdentities(IdentitySearchFactor searchFactor, string[] searchFactorValues,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] ReadIdentities(Guid[] teamFoundationIds, MembershipQuery queryMembership,
            ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters, IdentityPropertyScope propertyScope)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[][] ReadIdentities(TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor, string[] searchFactorValues,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(IdentityDescriptor descriptor, MembershipQuery queryMembership,
            ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters, IdentityPropertyScope propertyScope)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] ReadIdentities(IdentityDescriptor[] descriptors, MembershipQuery queryMembership,
            ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters, IdentityPropertyScope propertyScope)
        {
            throw new NotSupportedException();
        }
    }
}

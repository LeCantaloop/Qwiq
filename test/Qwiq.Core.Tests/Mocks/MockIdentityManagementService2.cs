using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace Microsoft.Qwiq.Core.Tests.Mocks
{
    public class MockIdentityManagementService2 : IIdentityManagementService2
    {
        private TeamFoundationIdentity[] GetNullIdentities()
        {
            return new TeamFoundationIdentity[]
            {
                null
            };
        }

        public TeamFoundationIdentity[] ReadIdentities(IdentityDescriptor[] descriptors, MembershipQuery queryMembership,
            ReadIdentityOptions readOptions)
        {
            return GetNullIdentities();
        }

        public TeamFoundationIdentity ReadIdentity(IdentityDescriptor descriptor, MembershipQuery queryMembership,
            ReadIdentityOptions readOptions)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity[] ReadIdentities(Guid[] teamFoundationIds, MembershipQuery queryMembership)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity[][] ReadIdentities(TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor, string[] searchFactorValues,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions)
        {
            return new []{ GetNullIdentities() };
        }

        public TeamFoundationIdentity ReadIdentity(TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor, string searchFactorValue,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity[][] ReadIdentities(IdentitySearchFactor searchFactor, string[] searchFactorValues,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity ReadIdentity(IdentitySearchFactor searchFactor, string searchFactorValue,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions)
        {
            throw new NotImplementedException();
        }

        public IdentityDescriptor CreateApplicationGroup(string scopeId, string groupName, string groupDescription)
        {
            throw new NotImplementedException();
        }

        public void DeleteApplicationGroup(IdentityDescriptor groupDescriptor)
        {
            throw new NotImplementedException();
        }

        public void AddMemberToApplicationGroup(IdentityDescriptor groupDescriptor, IdentityDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public void RemoveMemberFromApplicationGroup(IdentityDescriptor groupDescriptor, IdentityDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public bool IsMember(IdentityDescriptor groupDescriptor, IdentityDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public bool RefreshIdentity(IdentityDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public string GetScopeName(string scopeId)
        {
            throw new NotImplementedException();
        }

        public bool IsOwner(IdentityDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public bool IsOwnedWellKnownGroup(IdentityDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public string IdentityDomainScope
        {
            get { throw new NotImplementedException(); }
        }

        public void UpdateApplicationGroup(IdentityDescriptor groupDescriptor, GroupProperty groupProperty, string newValue)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity[] ListApplicationGroups(string scopeId, ReadIdentityOptions readOptions)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity[] GetMostRecentlyUsedUsers()
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity[] GetMostRecentlyUsedUsersEx(Guid teamId)
        {
            throw new NotImplementedException();
        }

        public void AddRecentUser(TeamFoundationIdentity identity)
        {
            throw new NotImplementedException();
        }

        public void AddRecentUser(Guid teamFoundationId)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity ReadIdentity(string generalSearchValue)
        {
            throw new NotImplementedException();
        }

        public FilteredIdentitiesList ReadFilteredIdentities(string expression, int suggestedPageSize, string lastSearchResult,
            bool lookForward, int queryMembership)
        {
            throw new NotImplementedException();
        }

        public void SetCustomDisplayName(string customDisplayName)
        {
            throw new NotImplementedException();
        }

        public void ClearCustomDisplayName()
        {
            throw new NotImplementedException();
        }

        public void UpdateExtendedProperties(TeamFoundationIdentity identity)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity ReadIdentity(TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor, string searchFactorValue,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity[] ListApplicationGroups(string scopeId, ReadIdentityOptions readOptions,
            IEnumerable<string> propertyNameFilters, IdentityPropertyScope propertyScope)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity ReadIdentity(IdentitySearchFactor searchFactor, string searchFactorValue,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity[][] ReadIdentities(IdentitySearchFactor searchFactor, string[] searchFactorValues,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity[] ReadIdentities(Guid[] teamFoundationIds, MembershipQuery queryMembership,
            ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters, IdentityPropertyScope propertyScope)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity[][] ReadIdentities(TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor, string[] searchFactorValues,
            MembershipQuery queryMembership, ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity ReadIdentity(IdentityDescriptor descriptor, MembershipQuery queryMembership,
            ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters, IdentityPropertyScope propertyScope)
        {
            throw new NotImplementedException();
        }

        public TeamFoundationIdentity[] ReadIdentities(IdentityDescriptor[] descriptors, MembershipQuery queryMembership,
            ReadIdentityOptions readOptions, IEnumerable<string> propertyNameFilters, IdentityPropertyScope propertyScope)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;

using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace Microsoft.Qwiq.Identity.Mocks
{
    public class MockIdentityManagementService2 : IIdentityManagementService2
    {
        private static readonly TeamFoundation.Framework.Client.TeamFoundationIdentity[] NullIdentities = { null };

        public string IdentityDomainScope => throw new NotSupportedException();

        public void AddMemberToApplicationGroup(
            TeamFoundation.Framework.Client.IdentityDescriptor groupDescriptor,
            TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public void AddRecentUser(TeamFoundation.Framework.Client.TeamFoundationIdentity identity)
        {
            throw new NotSupportedException();
        }

        public void AddRecentUser(Guid teamFoundationId)
        {
            throw new NotSupportedException();
        }

        public void ClearCustomDisplayName()
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.IdentityDescriptor CreateApplicationGroup(
            string scopeId,
            string groupName,
            string groupDescription)
        {
            throw new NotSupportedException();
        }

        public void DeleteApplicationGroup(TeamFoundation.Framework.Client.IdentityDescriptor groupDescriptor)
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

        public string GetScopeName(string scopeId)
        {
            throw new NotSupportedException();
        }

        public bool IsMember(
            TeamFoundation.Framework.Client.IdentityDescriptor groupDescriptor,
            TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public bool IsOwnedWellKnownGroup(TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public bool IsOwner(TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] ListApplicationGroups(
            string scopeId,
            ReadIdentityOptions readOptions)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] ListApplicationGroups(
            string scopeId,
            ReadIdentityOptions readOptions,
            IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            throw new NotSupportedException();
        }

        public FilteredIdentitiesList ReadFilteredIdentities(
            string expression,
            int suggestedPageSize,
            string lastSearchResult,
            bool lookForward,
            int queryMembership)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] ReadIdentities(
            TeamFoundation.Framework.Client.IdentityDescriptor[] descriptors,
            TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions)
        {
            return NullIdentities;
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] ReadIdentities(
            Guid[] teamFoundationIds,
            TeamFoundation.Framework.Common.MembershipQuery queryMembership)
        {
            return NullIdentities;
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[][] ReadIdentities(
            TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor,
            string[] searchFactorValues,
            TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions)
        {
            return new[] { NullIdentities };
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] ReadIdentities(
            Guid[] teamFoundationIds,
            TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions,
            IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            return NullIdentities;
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[][] ReadIdentities(
            TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor,
            string[] searchFactorValues,
            TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions,
            IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            return new[] { NullIdentities };
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[] ReadIdentities(
            TeamFoundation.Framework.Client.IdentityDescriptor[] descriptors,
            TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions,
            IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            return NullIdentities;
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(
            TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor,
            string searchFactorValue,
            TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions)
        {
            throw new NotImplementedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(
            TeamFoundation.Framework.Client.IdentityDescriptor descriptor,
            TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(string generalSearchValue)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(
            TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor,
            string searchFactorValue,
            TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions,
            IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(
            TeamFoundation.Framework.Client.IdentityDescriptor descriptor,
            TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions,
            IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            throw new NotSupportedException();
        }

        public bool RefreshIdentity(TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public void RemoveMemberFromApplicationGroup(
            TeamFoundation.Framework.Client.IdentityDescriptor groupDescriptor,
            TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public void SetCustomDisplayName(string customDisplayName)
        {
            throw new NotSupportedException();
        }

        public void UpdateApplicationGroup(
            TeamFoundation.Framework.Client.IdentityDescriptor groupDescriptor,
            GroupProperty groupProperty,
            string newValue)
        {
            throw new NotSupportedException();
        }

        public void UpdateExtendedProperties(TeamFoundation.Framework.Client.TeamFoundationIdentity identity)
        {
            throw new NotSupportedException();
        }

#pragma warning disable RECS0154 // Parameter is never used
        public void AddRecentUser(TeamFoundationIdentity identity)
#pragma warning restore RECS0154 // Parameter is never used
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[][] ReadIdentities(
#pragma warning disable RECS0154 // Parameter is never used
            IdentitySearchFactor searchFactor,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            string[] searchFactorValues,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            TeamFoundation.Framework.Common.MembershipQuery queryMembership,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            ReadIdentityOptions readOptions)
#pragma warning restore RECS0154 // Parameter is never used
        {
            return new[] { NullIdentities };
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity[][] ReadIdentities(
#pragma warning disable RECS0154 // Parameter is never used
            IdentitySearchFactor searchFactor,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            string[] searchFactorValues,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            TeamFoundation.Framework.Common.MembershipQuery queryMembership,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            ReadIdentityOptions readOptions,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            IEnumerable<string> propertyNameFilters,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            IdentityPropertyScope propertyScope)
#pragma warning restore RECS0154 // Parameter is never used
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(
#pragma warning disable RECS0154 // Parameter is never used
            IdentitySearchFactor searchFactor,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            string searchFactorValue,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            TeamFoundation.Framework.Common.MembershipQuery queryMembership,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            ReadIdentityOptions readOptions)
#pragma warning restore RECS0154 // Parameter is never used
        {
            throw new NotSupportedException();
        }

        public TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(
#pragma warning disable RECS0154 // Parameter is never used
            IdentitySearchFactor searchFactor,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            string searchFactorValue,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            TeamFoundation.Framework.Common.MembershipQuery queryMembership,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            ReadIdentityOptions readOptions,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            IEnumerable<string> propertyNameFilters,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            IdentityPropertyScope propertyScope)
#pragma warning restore RECS0154 // Parameter is never used
        {
            throw new NotSupportedException();
        }

#pragma warning disable RECS0154 // Parameter is never used
        public void UpdateExtendedProperties(TeamFoundationIdentity identity)
#pragma warning restore RECS0154 // Parameter is never used
        {
            throw new NotSupportedException();
        }
    }
}
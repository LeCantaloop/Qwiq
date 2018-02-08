using System;
using System.Collections.Generic;

using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace Qwiq.Identity.Mocks
{
    public class MockIdentityManagementService2 : IIdentityManagementService2
    {
        private static readonly Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity[] NullIdentities = { null };

        public string IdentityDomainScope => throw new NotSupportedException();

        public void AddMemberToApplicationGroup(
            Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor groupDescriptor,
            Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public void AddRecentUser(Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity identity)
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

        public Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor CreateApplicationGroup(
            string scopeId,
            string groupName,
            string groupDescription)
        {
            throw new NotSupportedException();
        }

        public void DeleteApplicationGroup(Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor groupDescriptor)
        {
            throw new NotSupportedException();
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity[] GetMostRecentlyUsedUsers()
        {
            throw new NotSupportedException();
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity[] GetMostRecentlyUsedUsersEx(Guid teamId)
        {
            throw new NotSupportedException();
        }

        public string GetScopeName(string scopeId)
        {
            throw new NotSupportedException();
        }

        public bool IsMember(
            Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor groupDescriptor,
            Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public bool IsOwnedWellKnownGroup(Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public bool IsOwner(Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity[] ListApplicationGroups(
            string scopeId,
            ReadIdentityOptions readOptions)
        {
            throw new NotSupportedException();
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity[] ListApplicationGroups(
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

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity[] ReadIdentities(
            Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor[] descriptors,
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions)
        {
            return NullIdentities;
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity[] ReadIdentities(
            Guid[] teamFoundationIds,
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership)
        {
            return NullIdentities;
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity[][] ReadIdentities(
            Microsoft.TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor,
            string[] searchFactorValues,
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions)
        {
            return new[] { NullIdentities };
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity[] ReadIdentities(
            Guid[] teamFoundationIds,
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions,
            IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            return NullIdentities;
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity[][] ReadIdentities(
            Microsoft.TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor,
            string[] searchFactorValues,
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions,
            IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            return new[] { NullIdentities };
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity[] ReadIdentities(
            Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor[] descriptors,
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions,
            IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            return NullIdentities;
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(
            Microsoft.TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor,
            string searchFactorValue,
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions)
        {
            throw new NotImplementedException();
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(
            Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor descriptor,
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions)
        {
            throw new NotSupportedException();
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(string generalSearchValue)
        {
            throw new NotSupportedException();
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(
            Microsoft.TeamFoundation.Framework.Common.IdentitySearchFactor searchFactor,
            string searchFactorValue,
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions,
            IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            throw new NotSupportedException();
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(
            Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor descriptor,
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership,
            ReadIdentityOptions readOptions,
            IEnumerable<string> propertyNameFilters,
            IdentityPropertyScope propertyScope)
        {
            throw new NotSupportedException();
        }

        public bool RefreshIdentity(Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public void RemoveMemberFromApplicationGroup(
            Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor groupDescriptor,
            Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor descriptor)
        {
            throw new NotSupportedException();
        }

        public void SetCustomDisplayName(string customDisplayName)
        {
            throw new NotSupportedException();
        }

        public void UpdateApplicationGroup(
            Microsoft.TeamFoundation.Framework.Client.IdentityDescriptor groupDescriptor,
            GroupProperty groupProperty,
            string newValue)
        {
            throw new NotSupportedException();
        }

        public void UpdateExtendedProperties(Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity identity)
        {
            throw new NotSupportedException();
        }

#pragma warning disable RECS0154 // Parameter is never used
        public void AddRecentUser(TeamFoundationIdentity identity)
#pragma warning restore RECS0154 // Parameter is never used
        {
            throw new NotSupportedException();
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity[][] ReadIdentities(
#pragma warning disable RECS0154 // Parameter is never used
            IdentitySearchFactor searchFactor,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            string[] searchFactorValues,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            ReadIdentityOptions readOptions)
#pragma warning restore RECS0154 // Parameter is never used
        {
            return new[] { NullIdentities };
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity[][] ReadIdentities(
#pragma warning disable RECS0154 // Parameter is never used
            IdentitySearchFactor searchFactor,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            string[] searchFactorValues,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership,
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

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(
#pragma warning disable RECS0154 // Parameter is never used
            IdentitySearchFactor searchFactor,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            string searchFactorValue,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            ReadIdentityOptions readOptions)
#pragma warning restore RECS0154 // Parameter is never used
        {
            throw new NotSupportedException();
        }

        public Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity ReadIdentity(
#pragma warning disable RECS0154 // Parameter is never used
            IdentitySearchFactor searchFactor,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            string searchFactorValue,
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning disable RECS0154 // Parameter is never used
            Microsoft.TeamFoundation.Framework.Common.MembershipQuery queryMembership,
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
using System.Collections.Generic;


namespace Qwiq.Identity
{
    /// <summary>
    /// Provides methods for managing and resolving identities in Team Foundation Server / Azure DevOps.
    /// </summary>
    public interface IIdentityManagementService
    {
        /// <summary>
        /// Creates an identity descriptor from the specified identity type and identifier.
        /// </summary>
        /// <param name="identityType">The type of identity (e.g., "Windows", "ServiceIdentity").</param>
        /// <param name="identifier">The unique identifier for the identity.</param>
        /// <returns>An <see cref="IIdentityDescriptor"/> representing the identity.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="identityType"/> or <paramref name="identifier"/> is null.</exception>
        IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier);

        /// <summary>
        /// Read identities for given <paramref name="descriptors"/>.
        /// </summary>
        /// <param name="descriptors">A set of <see cref="IIdentityDescriptor"/>s. Cannot be null.</param>
        /// <returns>An enumerable of identities. May contain null entries for descriptors that could not be resolved.</returns>

        IEnumerable<ITeamFoundationIdentity> ReadIdentities(IEnumerable<IIdentityDescriptor> descriptors);

        /// <summary>
        /// Read identities for given <paramref name="descriptors"/>.
        /// </summary>
        /// <param name="descriptors">A set of <see cref="IIdentityDescriptor"/>s. Cannot be null.</param>
        /// <param name="queryMembership">Specifies whether to query membership information.</param>
        /// <returns>An enumerable of identities. May contain null entries for descriptors that could not be resolved.</returns>

        IEnumerable<ITeamFoundationIdentity> ReadIdentities(IEnumerable<IIdentityDescriptor> descriptors, MembershipQuery queryMembership);

        /// <summary>
        /// Read identities for given <paramref name="searchFactor"/> and <paramref name="searchFactorValues"/>.
        /// </summary>
        /// <param name="searchFactor">Specific search.</param>
        /// <param name="searchFactorValues">Actual search strings. Cannot be null.</param>
        /// <returns>An enumerable set of identities corresponding 1 to 1 with <paramref name="searchFactorValues"/>. Values may be empty if no matches found.</returns>

        IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(
            IdentitySearchFactor searchFactor,
            IEnumerable<string> searchFactorValues);

        /// <summary>
        /// Read identities for given <paramref name="searchFactor"/> and <paramref name="searchFactorValues"/>.
        /// </summary>
        /// <param name="searchFactor">Specific search.</param>
        /// <param name="searchFactorValues">Actual search strings. Cannot be null.</param>
        /// <param name="queryMembership">Specifies whether to query membership information.</param>
        /// <returns>An enumerable set of identities corresponding 1 to 1 with <paramref name="searchFactorValues"/>. Values may be empty if no matches found.</returns>

        IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(
            IdentitySearchFactor searchFactor,
            IEnumerable<string> searchFactorValues,
            MembershipQuery queryMembership);

        /// <summary>
        /// Read a single identity for the given search criteria.
        /// </summary>
        /// <param name="searchFactor">The factor to search by.</param>
        /// <param name="searchFactorValue">The value to search for. Cannot be null.</param>
        /// <returns>The matching identity, or null if not found.</returns>
        ITeamFoundationIdentity? ReadIdentity(IdentitySearchFactor searchFactor, string searchFactorValue);

        /// <summary>
        /// Read a single identity for the given search criteria.
        /// </summary>
        /// <param name="searchFactor">The factor to search by.</param>
        /// <param name="searchFactorValue">The value to search for. Cannot be null.</param>
        /// <param name="queryMembership">Specifies whether to query membership information.</param>
        /// <returns>The matching identity, or null if not found.</returns>
        ITeamFoundationIdentity? ReadIdentity(IdentitySearchFactor searchFactor, string searchFactorValue, MembershipQuery queryMembership);
    }
}
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Qwiq.Identity
{
    public interface IIdentityManagementService
    {
        [NotNull]
        [Pure]
        IIdentityDescriptor CreateIdentityDescriptor([NotNull] string identityType, [NotNull] string identifier);

        /// <summary>
        /// Read identities for given <paramref name="descriptors"/>.
        /// </summary>
        /// <param name="descriptors">A set of <see cref="IIdentityDescriptor"/>s</param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        IEnumerable<ITeamFoundationIdentity> ReadIdentities([NotNull] IEnumerable<IIdentityDescriptor> descriptors);

        /// <summary>
        /// Read identities for given <paramref name="descriptors"/>.
        /// </summary>
        /// <param name="descriptors">A set of <see cref="IIdentityDescriptor"/>s</param>
        /// <param name="queryMembership"></param>
        /// <returns></returns>
        [NotNull]
        [Pure]
        IEnumerable<ITeamFoundationIdentity> ReadIdentities([NotNull] IEnumerable<IIdentityDescriptor> descriptors, MembershipQuery queryMembership);

        /// <summary>
        /// Read identities for given <paramref name="searchFactor"/> and <paramref name="searchFactorValues"/>.
        /// </summary>
        /// <param name="searchFactor">Specific search.</param>
        /// <param name="searchFactorValues">Actual search strings.</param>
        /// <returns>An enumerable set of identities corresponding 1 to 1 with <paramref name="searchFactorValues"/>.</returns>
        [NotNull]
        [Pure]
        IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(
            IdentitySearchFactor searchFactor,
            [NotNull] IEnumerable<string> searchFactorValues);

        /// <summary>
        /// Read identities for given <paramref name="searchFactor"/> and <paramref name="searchFactorValues"/>.
        /// </summary>
        /// <param name="searchFactor">Specific search.</param>
        /// <param name="searchFactorValues">Actual search strings.</param>
        /// <param name="queryMembership"></param>
        /// <returns>An enumerable set of identities corresponding 1 to 1 with <paramref name="searchFactorValues"/>.</returns>
        [NotNull]
        [Pure]
        IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(
            IdentitySearchFactor searchFactor,
            [NotNull] IEnumerable<string> searchFactorValues,
            MembershipQuery queryMembership);

        [CanBeNull]
        [Pure]
        ITeamFoundationIdentity ReadIdentity(IdentitySearchFactor searchFactor, [NotNull] string searchFactorValue);

        [CanBeNull]
        [Pure]
        ITeamFoundationIdentity ReadIdentity(IdentitySearchFactor searchFactor, [NotNull] string searchFactorValue, MembershipQuery queryMembership);
    }
}
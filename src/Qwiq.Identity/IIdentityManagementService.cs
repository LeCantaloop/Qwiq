using System.Collections.Generic;

namespace Microsoft.Qwiq.Identity
{
    public interface IIdentityManagementService
    {
        IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier);

        /// <summary>
        /// Read identities for given <paramref name="descriptors"/>.
        /// </summary>
        /// <param name="descriptors">A set of <see cref="IIdentityDescriptor"/>s</param>
        /// <returns></returns>
        IEnumerable<ITeamFoundationIdentity> ReadIdentities(IEnumerable<IIdentityDescriptor> descriptors);

        /// <summary>
        /// Read identities for given <paramref name="searchFactor"/> and <paramref name="searchFactorValues"/>.
        /// </summary>
        /// <param name="searchFactor">Specific search.</param>
        /// <param name="searchFactorValues">Actual search strings.</param>
        /// <returns>An enumerable set of identities corresponding 1 to 1 with <paramref name="searchFactorValues"/>.</returns>
        IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(
            IdentitySearchFactor searchFactor,
            IEnumerable<string> searchFactorValues);

        ITeamFoundationIdentity ReadIdentity(IdentitySearchFactor searchFactor, string searchFactorValue);
    }
}
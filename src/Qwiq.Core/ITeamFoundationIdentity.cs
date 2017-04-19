using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface ITeamFoundationIdentity
    {
        /// <summary>
        ///     Gets the unique identifier for the identity's provider.
        /// </summary>
        /// <value>The descriptor.</value>
        IIdentityDescriptor Descriptor { get; }

        /// <summary>
        ///     Gets the full name of the identity for display purposes. The display name can come from the identity provider, or
        ///     may have been set as a custom display name within TFS.
        /// </summary>
        /// <value>The display name.</value>
        string DisplayName { get; }

        /// <summary>
        ///     Gets a value indicating whether the identity is current with the provider.
        /// </summary>
        /// <value><c>true</c> if this instance is current; otherwise, <c>false</c>.</value>
        bool IsActive { get; }

        /// <summary>
        ///     Indicates that the identity is a group, possibly containing other identities as members.
        /// </summary>
        bool IsContainer { get; }

        /// <summary>
        ///     Gets the set of <see cref="IIdentityDescriptor" /> of groups containing this identity.
        /// </summary>
        /// <value>The member of.</value>
        IEnumerable<IIdentityDescriptor> MemberOf { get; }

        /// <summary>
        ///     Gets the set of <see cref="IIdentityDescriptor" />s for members of this identity.
        /// </summary>
        /// <value>The members.</value>
        IEnumerable<IIdentityDescriptor> Members { get; }

        /// <summary>
        ///     Gets the team foundation identifier.
        /// </summary>
        /// <value>The team foundation identifier.</value>
        Guid TeamFoundationId { get; }

        /// <summary>
        ///     Gets the unique name of this identity.
        /// </summary>
        /// <remarks>
        ///     The unique name is a combination of the domain and account name properties of the identity and the
        ///     <see cref="UniqueUserId" />.
        ///     If the current user is active: and there is no domain, then the unique name equals the account name; otherwise, the
        ///     unique name equals DOMAIN\ACCOUNTNAME.
        ///     If the current user is not active and there is no domain, then the unique name equals the account name and the
        ///     <see cref="UniqueUserId" />; otherwise, the unique name equals DOMAIN\ACCOUNTNAME:UniqueUserId
        /// </remarks>
        /// <example>
        ///     DanJ
        ///     DanJ:1
        ///     CONTOSO\DanJ
        ///     CONTOSO\DanJ:1
        /// </example>
        /// <value>The unique name of the identity.</value>
        string UniqueName { get; }

        /// <summary>
        ///     Gets the unique user identifier used to distinguish deleted accounts from one another.
        /// </summary>
        /// <remarks>
        ///     If the current user is active (e.g. not deleted), the value is equal to
        ///     <see cref="VisualStudio.Services.Common.IdentityConstants.ActiveUniqueId" />.
        /// </remarks>
        /// <value>The unique user identifier.</value>
        int UniqueUserId { get; }

        /// <summary>
        /// Attribute accessor. Will return the caller supplied default value if attribute
        /// is not present (will not throw).
        /// </summary>
        string GetAttribute(string name, string defaultValue);

        /// <summary>Property accessor. Will throw if not found.</summary>
        object GetProperty(string name);

        /// <summary>
        /// Property bag. This could be useful, for example if consumer has
        /// to iterate through current properties and modify / remove
        /// some based on pattern matching property names.
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> GetProperties();
    }
}
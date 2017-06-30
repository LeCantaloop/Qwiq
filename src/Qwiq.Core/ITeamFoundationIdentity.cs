using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    [ContractClass(typeof(TeamFoundationIdentityContract))]
    public interface ITeamFoundationIdentity
    {
        /// <summary>
        ///     Gets the unique identifier for the identity's provider.
        /// </summary>
        /// <value>The descriptor.</value>
        [NotNull]
        IIdentityDescriptor Descriptor { get; }

        /// <summary>
        ///     Gets the full name of the identity for display purposes. The display name can come from the identity provider (e.g. Active Directory), or
        ///     may have been set as a custom display name within TFS.
        /// </summary>
        /// <remarks>
        /// If the identity provider does not supply a full name, and no custom display name is set, another property like account name or email address will be used as the display name.
        /// </remarks>
        [NotNull]
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
        [NotNull]
        IEnumerable<IIdentityDescriptor> MemberOf { get; }

        /// <summary>
        ///     Gets the set of <see cref="IIdentityDescriptor" />s for members of this identity.
        /// </summary>
        /// <value>The members.</value>
        [NotNull]
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
        [NotNull]
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

    [ContractClassFor(typeof(ITeamFoundationIdentity))]
    internal abstract class TeamFoundationIdentityContract : ITeamFoundationIdentity
    {
        public IIdentityDescriptor Descriptor
        {
            get
            {
                Contract.Ensures(Contract.Result<IIdentityDescriptor>() != null);

                return default(IIdentityDescriptor);
            }
        }

        /// <inheritdoc />
        public abstract bool IsContainer { get; }

        public IEnumerable<IIdentityDescriptor> MemberOf
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IIdentityDescriptor>>() != null);

                return default(IEnumerable<IIdentityDescriptor>);
            }
        }

        public IEnumerable<IIdentityDescriptor> Members
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IIdentityDescriptor>>() != null);

                return default(IEnumerable<IIdentityDescriptor>);
            }
        }

        /// <inheritdoc />
        public abstract Guid TeamFoundationId { get; }

        public string DisplayName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

                return default(string);
            }
        }

        /// <inheritdoc />
        public abstract bool IsActive { get; }

        public string UniqueName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

                return default(string);
            }
        }

        /// <inheritdoc />
        public abstract int UniqueUserId { get; }

        /// <inheritdoc />
        public abstract string GetAttribute(string name, string defaultValue);

        /// <inheritdoc />
        public abstract object GetProperty(string name);

        /// <inheritdoc />
        public abstract IEnumerable<KeyValuePair<string, object>> GetProperties();
    }
}
namespace Qwiq
{
    public enum IdentitySearchFactor
    {
        /// <summary>
        ///     NT account name (domain\alias or alias@domain.tld)
        /// </summary>
        AccountName = 0,

        /// <summary>
        ///     Display name
        /// </summary>
        DisplayName = 1,

        /// <summary>
        ///     Find project admin group
        /// </summary>
        AdministratorsGroup = 2,

        /// <summary>
        ///     Find the identity using the identifier
        /// </summary>
        /// <see cref="IIdentityDescriptor.Identifier" />
        Identifier = 3,

        /// <summary>
        ///     Email address
        /// </summary>
        MailAddress = 4,

        /// <summary>
        ///     A general search for identity
        /// </summary>
        /// <remarks>
        ///     This is the default search factor for shorter overloads of <see cref="IIdentityManagementService.ReadIdentity" />,
        ///     and typically the correct choice for user input. Use the general search factor to find one or more identities by
        ///     one of the following properties:
        ///     - Display name
        ///     - Account name
        ///     - Unique name
        ///     Unique name may be easier to type than display name for users. It can also be used to indicate a single identity
        ///     when two or more identities share the same display name (e.g. "John Smith")
        /// </remarks>
        /// <seealso cref="DisplayName" />
        /// <see cref="AccountName" />
        General = 5,

        /// <summary>
        ///     Alternate login name
        /// </summary>
        Alias = 6
    }
}
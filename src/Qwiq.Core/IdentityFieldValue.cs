using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.Qwiq
{
    /// <summary>
    ///     Class IdentityFieldValue.
    /// </summary>
    public class IdentityFieldValue
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="IdentityFieldValue" /> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        public IdentityFieldValue(ITeamFoundationIdentity identity)
            : this(identity.DisplayName, identity.Descriptor.Identifier, identity.TeamFoundationId.ToString())
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IdentityFieldValue" /> class.
        /// </summary>
        /// <param name="displayName">The display name (e.g. "Chris Johnson &lt;chrisjohns@contoso.com&gt;").</param>
        /// <param name="fullName">
        ///     The value from the descriptor identifier (e.g.
        ///     CD4C5751-F4E6-41D5-A4C9-EFFD66BC8E9C\chrisjohns@contoso.com).
        /// </param>
        /// <param name="sid">The security identifier (SID) for the identity.</param>
        public IdentityFieldValue(string displayName, string fullName, string sid)
            : this(displayName)
        {
            Sid = sid;
            FullName = fullName;

            var arr = FullName.Split(IdentityConstants.DomainAccountNameSeparator);
            if (arr.Length != 2 || arr[1] == Sid) return;

            if (arr[1].Contains("@"))
            {
                Email = arr[1];
                Alias = arr[1].Split('@')[0];

                if (Guid.TryParse(arr[0], out Guid guid)) Domain = arr[0];
            }
            else
            {
                if (Guid.TryParse(arr[0], out Guid guid))
                {
                    Alias = arr[1];
                }
                else
                {
                    Domain = arr[0];
                    Alias = arr[1];
                }
            }
        }

        public IdentityFieldValue(string displayName)
        {
            if (!string.IsNullOrEmpty(displayName))
                if (displayName.Contains("<"))
                {
                    var arr = displayName.Split('<');
                    if (arr[1].Contains("@"))
                    {
                        Email = arr[1].Trim('>');
                        Alias = Email.Split('@')[0];
                    }
                }

            DisplayPart = displayName;
        }

        /// <summary>
        ///     Gets the alias.
        /// </summary>
        /// <value>The alias parsed from <see cref="FullName" />.</value>
        public string Alias { get; }

        /// <summary>
        ///     Gets the display name.
        /// </summary>
        /// <value>The display name without the account name, if it exists.</value>
        public string DisplayName => !string.IsNullOrEmpty(DisplayPart) ? DisplayPart.Split('<')[0].Trim() : DisplayPart;

        /// <summary>
        ///     Gets the display part.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayPart { get; }

        /// <summary>
        ///     Gets the domain.
        /// </summary>
        /// <value>The domain.</value>
        public string Domain { get; }

        /// <summary>
        ///     Gets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; }

        /// <summary>
        ///     Gets the full name.
        /// </summary>
        /// <value>The full name as determined by the descriptor identifier.</value>
        public string FullName { get; }

        /// <summary>
        ///     Gets the name of the identity.
        /// </summary>
        /// <value>
        ///     The <see cref="Email" /> if it exists, the qualified <see cref="Domain" />\<see cref="Alias" /> if it exists,
        ///     the <see cref="Alias" />if it exists, or empty.
        /// </value>
        public string IdentityName
        {
            get
            {
                if (!string.IsNullOrEmpty(Email)) return Email;
                if (!string.IsNullOrEmpty(Domain)) return string.Format(CultureInfo.InvariantCulture, IdentityConstants.DomainQualifiedAccountNameFormat, Domain, Alias);
                if (!string.IsNullOrEmpty(Alias)) return Alias;

                return string.Empty;
            }
        }

        /// <summary>
        ///     Gets the Security Identifier (SID).
        /// </summary>
        /// <value>The SID.</value>
        public string Sid { get; }
    }
}
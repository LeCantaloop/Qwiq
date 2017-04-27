using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Qwiq
{
    /// <summary>
    ///     Represents an identity
    /// </summary>
    public class IdentityFieldValue
    {
        // "Chris Johnson <chrisjohns@contoso.com>"
        private static readonly Regex AccountNameRegex = new Regex("^.+<(.+@.+)>$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // "Chris Johnson"
        private static readonly Regex DisplayNameRegex =
                new Regex(@"^[^<\\]*(?:<[^>]*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // "Chris Johnson <CONTOSO\chrisjohns>"
        private static readonly Regex DomainAccountRegex = new Regex(@"^.+<(.+\\.+)>$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex ScopeRegex = new Regex(
                                                             @"^\[[0-9A-Za-z ]+\]\\(.+)<([0-9A-Fa-f]{8}(?:-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12})>$",
                                                             RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex VsidRegex = new Regex(
                                                            @"^\[[0-9A-Za-z ]+\]\\(.+)<id:([0-9A-Fa-f]{8}(?:-[0-9A-Fa-f]{4}){3}-[0-9A-Fa-f]{12})>$",
                                                            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        ///     Initializes a new instance of the <see cref="IdentityFieldValue" /> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <exception cref="ArgumentNullException">identity</exception>
        public IdentityFieldValue(ITeamFoundationIdentity identity)
            : this(identity?.DisplayName, identity?.Descriptor?.Identifier, identity?.TeamFoundationId.ToString())
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IdentityFieldValue" /> class.
        /// </summary>
        /// <param name="displayName">The display name (e.g. "Chris Johnson &lt;chrisjohns@contoso.com&gt;").</param>
        /// <param name="identifier">
        ///     The value from the descriptor identifier (e.g.
        ///     CD4C5751-F4E6-41D5-A4C9-EFFD66BC8E9C\chrisjohns@contoso.com).
        /// </param>
        /// <param name="teamFoundationId">The security identifier (SID) for the identity.</param>
        public IdentityFieldValue(string displayName, string identifier, string teamFoundationId)
            : this(displayName)
        {
            Identifier = identifier;

            if (!string.IsNullOrEmpty(teamFoundationId) && Guid.TryParse(teamFoundationId, out Guid tfsid))
                TeamFoundationId = teamFoundationId;

            var arr = Identifier.Split(IdentityConstants.DomainAccountNameSeparator);
            if (arr.Length != 2 || arr[1] == TeamFoundationId) return;

            if (arr[1].Contains("@"))
            {
                Email = arr[1];
                LogonName = arr[1].Split('@')[0];
                AccountName = arr[1];
                if (Guid.TryParse(arr[0], out Guid guid)) Domain = arr[0];
            }
            else
            {
                if (Guid.TryParse(arr[0], out Guid guid))
                {
                    LogonName = arr[1];
                }
                else
                {
                    Domain = arr[0];
                    LogonName = arr[1];
                }
            }
        }

        public IdentityFieldValue(string displayName)
        {
            DisplayPart = displayName;

            if (!string.IsNullOrEmpty(displayName))
            {
                if (TryGetVsid(displayName, out Guid guid2, out string str))
                {
                    DisplayPart = str;
                    return;
                }
                if (TryGetDomainAndAccountName(displayName, out string str2))
                {
                    AccountName = str2;

                    var strArray = str2.Split(IdentityConstants.DomainAccountNameSeparator);
                    if (strArray.Length != 2) return;

                    Domain = strArray[0];
                    LogonName = strArray[1];

                    return;
                }
                if (TryGetAccountName(displayName, out str2))
                {
                    AccountName = str2;
                    if (str2.Contains("@"))
                    {
                        Email = str2;
                        LogonName = str2.Split('@')[0];
                    }
                    DisplayPart = displayName;
                    return;
                }
                if (TryGetDisplayName(displayName, out str2)) DisplayPart = str2;
            }
        }

        /// <summary>
        ///     Gets the the User principal name (UPN) or the down-level login name.
        /// </summary>
        /// <remarks>
        ///     This can be in the UPN format (e.g. UserName@Example.Microsoft.com) or the down-level logon name format (e.g.
        ///     EXAMPLE\UserName).
        /// </remarks>
        public string AccountName { get; }

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
        /// <seealso cref="IIdentityDescriptor.Identifier" />
        public string Identifier { get; }

        /// <summary>
        ///     Gets the name of the identity.
        /// </summary>
        /// <value>
        ///     The <see cref="Email" /> if it exists, the qualified <see cref="Domain" />\<see cref="LogonName" /> if it exists,
        ///     the <see cref="LogonName" />if it exists, or empty.
        /// </value>
        public string IdentityName
        {
            get
            {
                if (!string.IsNullOrEmpty(Email)) return Email;
                if (!string.IsNullOrEmpty(Domain))
                    return string.Format(
                                         CultureInfo.InvariantCulture,
                                         IdentityConstants.DomainQualifiedAccountNameFormat,
                                         Domain,
                                         LogonName);
                if (!string.IsNullOrEmpty(LogonName)) return LogonName;

                return null;
            }
        }

        /// <summary>
        ///     Gets the user account (logon) name.
        /// </summary>
        /// <value>The logon name parsed from <see cref="Identifier" />, User Principal Name, or down-level logon name.</value>
        public string LogonName { get; }

        public string TeamFoundationId { get; }

        /// <summary>
        /// Performs an explicit conversion from <see cref="IdentityFieldValue"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>If <paramref name="value"/> is null, null; otherwise, <see cref="IdentityName"/>.</returns>
        public static explicit operator string(IdentityFieldValue value)
        {
            if (value == null) return null;
            return value.IdentityName;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.IsNullOrEmpty(IdentityName)
                       ? DisplayName
                       : $"{DisplayName} <{AccountName}>".ToString(CultureInfo.InvariantCulture);
        }

        private static bool TryGetAccountName(string search, out string acccountName)
        {
            var match = AccountNameRegex.Match(search);
            acccountName = null;
            if (match.Success && match.Groups.Count > 1)
            {
                acccountName = match.Groups[1].Value;
                return true;
            }
            return false;
        }

        private static bool TryGetDisplayName(string search, out string displayName)
        {
            var match = DisplayNameRegex.Match(search);
            displayName = null;
            if (match.Success && match.Groups.Count > 0)
            {
                displayName = match.Groups[0].Value;
                return true;
            }
            return false;
        }

        private static bool TryGetDomainAndAccountName(string search, out string domainAndAcccountName)
        {
            var match = DomainAccountRegex.Match(search);
            domainAndAcccountName = null;
            if (match.Success && match.Groups.Count > 1 && match.Groups[1].Value.Contains(@"\"))
            {
                domainAndAcccountName = match.Groups[1].Value;
                return true;
            }
            return false;
        }

        private static bool TryGetScope(string search, out Guid scopeId, out string displayName)
        {
            var match = ScopeRegex.Match(search);
            if (match.Success && match.Groups.Count > 1)
            {
                displayName = match.Groups[1].Value;
                Guid.TryParse(match.Groups[2].Value, out scopeId);
                return true;
            }
            scopeId = Guid.Empty;
            displayName = string.Empty;
            return false;
        }

        private static bool TryGetVsid(string search, out Guid vsid, out string displayName)
        {
            var match = VsidRegex.Match(search);
            if (match.Success && match.Groups.Count > 1)
            {
                displayName = match.Groups[1].Value;
                Guid.TryParse(match.Groups[2].Value, out vsid);
                return true;
            }
            vsid = Guid.Empty;
            displayName = string.Empty;
            return false;
        }
    }
}
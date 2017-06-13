using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Mocks
{
    public class MockTeamFoundationIdentity : TeamFoundationIdentity
    {
        private readonly IDictionary<string, object> _properties;

        public MockTeamFoundationIdentity(
            IIdentityDescriptor descriptor,
            string displayName,
            Guid teamFoundationId,
            bool isActive = true,
            IEnumerable<IIdentityDescriptor> members = null,
            IEnumerable<IIdentityDescriptor> memberOf = null)
            : base(isActive, teamFoundationId == Guid.Empty ? Guid.NewGuid() : teamFoundationId, isActive ? 0 : 1)

        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            MemberOf = memberOf ?? ZeroLengthArrayOfIdentityDescriptor;
            Members = members ?? ZeroLengthArrayOfIdentityDescriptor;
            IsContainer = false;
            Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));

            var f = new IdentityFieldValue(DisplayName, Descriptor.Identifier, TeamFoundationId.ToString());
            _properties =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        { IdentityAttributeTags.SchemaClassName, "User"},
                        { IdentityAttributeTags.Description, string.Empty },
                        { IdentityAttributeTags.Domain, f.Domain },
                        { IdentityAttributeTags.AccountName, f.AccountName },
                        { IdentityAttributeTags.DistinguishedName, string.Empty},
                        { IdentityAttributeTags.MailAddress, f.Email },
                        { IdentityAttributeTags.SpecialType, "Generic" },
                        { IdentityAttributeTags.IdentityTypeClaim, Descriptor.IdentityType }
                    };
        }

        [Obsolete("This method has been deprecated and will be removed in a future release.")]
        public MockTeamFoundationIdentity(string displayName, string uniqueName)
            : this(MockIdentityDescriptor.Create(uniqueName), displayName, Guid.Empty)
        {
        }

        public sealed override IIdentityDescriptor Descriptor { get; }

        public sealed override string DisplayName { get; }

        public sealed override bool IsContainer { get; }

        public sealed override IEnumerable<IIdentityDescriptor> MemberOf { get; }

        public sealed override IEnumerable<IIdentityDescriptor> Members { get; }

        public override string GetAttribute(string name, string defaultValue)
        {
            return _properties.TryGetValue(name, out object obj)
                       ? obj?.ToString() ?? defaultValue
                       : defaultValue;
        }

        public override IEnumerable<KeyValuePair<string, object>> GetProperties()
        {
            return _properties;
        }

        public override object GetProperty(string name)
        {
            return _properties[name];
        }
    }
}
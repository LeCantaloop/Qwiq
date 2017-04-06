using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Mocks
{
    public class MockTeamFoundationIdentity : TeamFoundationIdentity
    {
        private readonly IDictionary<string, object> _properties;

        private bool _isActive;

        public MockTeamFoundationIdentity(string displayName, string uniqueName)
        {
            _properties =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Account", uniqueName },
                        {
                            "Domain",
                            MockIdentityDescriptor.Domain
                        }
                    };

            DisplayName = displayName;
            MemberOf = Enumerable.Empty<IIdentityDescriptor>();
            Members = Enumerable.Empty<IIdentityDescriptor>();
            IsActive = true;
            IsContainer = false;
            TeamFoundationId = Guid.Empty;
            Descriptor = new MockIdentityDescriptor(uniqueName);
            UniqueUserId = IsActive ? 0 : 1;
        }

        public sealed override IIdentityDescriptor Descriptor { get; internal set; }

        public override string DisplayName { get; }

        public new bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                UniqueUserId = value ? 0 : 1;
            }
        }

        public sealed override bool IsContainer { get; }

        public sealed override IEnumerable<IIdentityDescriptor> MemberOf { get; }

        public sealed override IEnumerable<IIdentityDescriptor> Members { get; }

        public sealed override Guid TeamFoundationId { get; internal set; }

        public new int UniqueUserId { get; private set; }

        public override string GetAttribute(string name, string defaultValue)
        {
            return _properties.TryGetValue(name, out object obj) ? obj.ToString() : defaultValue;
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
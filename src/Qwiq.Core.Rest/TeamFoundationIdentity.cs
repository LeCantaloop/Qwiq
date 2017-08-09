using JetBrains.Annotations;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Microsoft.Qwiq.Client.Rest
{
    internal class TeamFoundationIdentity : Qwiq.TeamFoundationIdentity
    {
        private readonly Lazy<IIdentityDescriptor> _descriptor;

        private readonly Identity _identity;

        internal TeamFoundationIdentity([NotNull] Identity identity)
            : base(
                  identity.IsActive,
                  identity.Id,
                  identity.UniqueUserId,
                  identity.MemberOf.Select(item => item.AsProxy()).ToArray(),
                  identity.Members.Select(item => item.AsProxy()).ToArray())
        {
            Contract.Requires(identity != null);

            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            DisplayName = identity.DisplayName;
            IsContainer = identity.IsContainer;
            _descriptor = new Lazy<IIdentityDescriptor>(() => identity.Descriptor.AsProxy());
        }

        public override IIdentityDescriptor Descriptor => _descriptor.Value;

        public override string DisplayName { get; }

        public override bool IsContainer { get; }

        public override string GetAttribute(string name, string defaultValue)
        {
            if (_identity.Properties.TryGetValue(name, out object obj)) return obj?.ToString() ?? defaultValue;
            return defaultValue;
        }

        public override IEnumerable<KeyValuePair<string, object>> GetProperties()
        {
            return _identity.Properties;
        }

        public override object GetProperty(string name)
        {
            return _identity.Properties[name];
        }

        public override string ToString()
        {
            return UniqueName;
        }
    }
}
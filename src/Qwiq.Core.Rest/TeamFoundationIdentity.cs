using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;
using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.Qwiq.Rest
{
    internal class TeamFoundationIdentity : Qwiq.TeamFoundationIdentity
    {
        private readonly Lazy<IIdentityDescriptor> _descriptor;

        private readonly Identity _identity;

        private readonly Lazy<IEnumerable<IIdentityDescriptor>> _memberOf;

        private readonly Lazy<IEnumerable<IIdentityDescriptor>> _members;

        internal TeamFoundationIdentity(Identity identity)
            : base(identity.IsActive, identity.Id, identity.UniqueUserId)
        {
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            DisplayName = identity.DisplayName;

            IsContainer = identity.IsContainer;

            _descriptor = new Lazy<IIdentityDescriptor>(
                () => ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(
                    new IdentityDescriptor(identity.Descriptor)));
            _memberOf = new Lazy<IEnumerable<IIdentityDescriptor>>(
                () => identity.MemberOf.Select(
                    item => ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(
                        new IdentityDescriptor(item))));

            _members = new Lazy<IEnumerable<IIdentityDescriptor>>(
                () => identity.Members.Select(
                    item => ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(
                        new IdentityDescriptor(item))));
        }

        public override IIdentityDescriptor Descriptor => _descriptor.Value;

        public override string DisplayName { get; }

        public override bool IsContainer { get; }

        public override IEnumerable<IIdentityDescriptor> MemberOf => _memberOf.Value;

        public override IEnumerable<IIdentityDescriptor> Members => _members.Value;

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
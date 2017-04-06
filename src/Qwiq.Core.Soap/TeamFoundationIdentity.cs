using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class TeamFoundationIdentity : Qwiq.TeamFoundationIdentity
    {
        private readonly Lazy<IIdentityDescriptor> _descriptor;

        private readonly Tfs.TeamFoundationIdentity _identity;

        private readonly Lazy<IEnumerable<IIdentityDescriptor>> _memberOf;

        private readonly Lazy<IEnumerable<IIdentityDescriptor>> _members;

        internal TeamFoundationIdentity(Tfs.TeamFoundationIdentity identity)
            : base(identity.IsActive, identity.TeamFoundationId, identity.UniqueUserId)
        {
            _identity = identity;

            _descriptor = new Lazy<IIdentityDescriptor>(
                () => ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(
                    new IdentityDescriptor(_identity.Descriptor)));

            _memberOf = new Lazy<IEnumerable<IIdentityDescriptor>>(
                () => _identity.MemberOf.Select(
                    item => ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(
                        new IdentityDescriptor(item))));

            _members = new Lazy<IEnumerable<IIdentityDescriptor>>(
                () => _identity.Members.Select(
                    item => ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(
                        new IdentityDescriptor(item))));
        }

        public override IIdentityDescriptor Descriptor => _descriptor.Value;

        public override string DisplayName => _identity.DisplayName;

        public override bool IsActive => _identity.IsActive;

        public override bool IsContainer => _identity.IsContainer;

        public override IEnumerable<IIdentityDescriptor> MemberOf => _memberOf.Value;

        public override IEnumerable<IIdentityDescriptor> Members => _members.Value;

        public override Guid TeamFoundationId => _identity.TeamFoundationId;

        public override string UniqueName => _identity.UniqueName;

        public override int UniqueUserId => _identity.UniqueUserId;

        public override string GetAttribute(string name, string defaultValue)
        {
            return _identity.GetAttribute(name, defaultValue);
        }

        public override IEnumerable<KeyValuePair<string, object>> GetProperties()
        {
            return _identity.GetProperties();
        }

        public override object GetProperty(string name)
        {
            return _identity.GetProperty(name);
        }
    }
}
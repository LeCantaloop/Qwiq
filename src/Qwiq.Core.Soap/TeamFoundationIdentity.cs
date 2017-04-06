using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class TeamFoundationIdentity : ITeamFoundationIdentity
    {
        private readonly Lazy<IIdentityDescriptor> _descriptor;

        private readonly Tfs.TeamFoundationIdentity _identity;

        private readonly Lazy<IEnumerable<IIdentityDescriptor>> _memberOf;

        private readonly Lazy<IEnumerable<IIdentityDescriptor>> _members;

        internal TeamFoundationIdentity(Tfs.TeamFoundationIdentity identity)
        {
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));

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

        public IIdentityDescriptor Descriptor => _descriptor.Value;

        public string DisplayName => _identity.DisplayName;

        public bool IsActive => _identity.IsActive;

        public bool IsContainer => _identity.IsContainer;

        public IEnumerable<IIdentityDescriptor> MemberOf => _memberOf.Value;

        public IEnumerable<IIdentityDescriptor> Members => _members.Value;

        public Guid TeamFoundationId => _identity.TeamFoundationId;

        public string UniqueName => _identity.UniqueName;

        public int UniqueUserId => _identity.UniqueUserId;

        public string GetAttribute(string name, string defaultValue)
        {
            return _identity.GetAttribute(name, defaultValue);
        }

        public IEnumerable<KeyValuePair<string, object>> GetProperties()
        {
            return _identity.GetProperties();
        }

        public object GetProperty(string name)
        {
            return ExceptionHandlingDynamicProxyFactory.Create(_identity.GetProperty(name));
        }
    }
}
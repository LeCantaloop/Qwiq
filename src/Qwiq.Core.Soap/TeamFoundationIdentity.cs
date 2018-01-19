using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.Qwiq.Client.Soap
{
    internal class TeamFoundationIdentity : Qwiq.TeamFoundationIdentity
    {
        private readonly Lazy<IIdentityDescriptor> _descriptor;

        private readonly Tfs.TeamFoundationIdentity _identity;



        internal TeamFoundationIdentity(Tfs.TeamFoundationIdentity identity)
            : base(
                  identity.IsActive,
                  identity.TeamFoundationId,
                  identity.UniqueUserId,
                  identity.MemberOf.Select(i => new IdentityDescriptor(i)).ToArray(),
                  identity.Members.Select(i => new IdentityDescriptor(i)).ToArray())
        {
            _identity = identity;

            _descriptor = new Lazy<IIdentityDescriptor>(
                () => ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(
                    new IdentityDescriptor(_identity.Descriptor)));
        }

        public override IIdentityDescriptor Descriptor => _descriptor.Value;

        public override string DisplayName => _identity.DisplayName;

        public override bool IsContainer => _identity.IsContainer;

        public override string UniqueName => _identity.UniqueName;

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
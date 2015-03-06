using System;
using System.Collections.Generic;
using System.Linq;
using Tfs = Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.IE.Qwiq.Proxies
{
    public class TeamFoundationIdentityProxy : ITeamFoundationIdentity
    {
        private readonly Tfs.TeamFoundationIdentity _identity;

        internal TeamFoundationIdentityProxy(Tfs.TeamFoundationIdentity identity)
        {
            _identity = identity;
        }

        public IIdentityDescriptor Descriptor
        {
            get { return new IdentityDescriptorProxy(_identity.Descriptor); }
        }

        public string DisplayName
        {
            get { return _identity.DisplayName; }
        }

        public bool IsActive
        {
            get { return _identity.IsActive; }
        }

        public bool IsContainer
        {
            get { return _identity.IsContainer; }
        }

        public IEnumerable<IIdentityDescriptor> MemberOf
        {
            get { return _identity.MemberOf.Select(item => new IdentityDescriptorProxy(item)); }
        }

        public IEnumerable<IIdentityDescriptor> Members
        {
            get { return _identity.Members.Select(item => new IdentityDescriptorProxy(item)); }
        }

        public Guid TeamFoundationId
        {
            get { return _identity.TeamFoundationId; }
        }

        public string UniqueName
        {
            get { return _identity.UniqueName; }
        }

        public int UniqueUserId
        {
            get { return _identity.UniqueUserId; }
        }
    }
}
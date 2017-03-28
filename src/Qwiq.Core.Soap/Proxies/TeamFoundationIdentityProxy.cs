using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    public class TeamFoundationIdentityProxy : ITeamFoundationIdentity
    {
        private readonly Tfs.TeamFoundationIdentity _identity;

        private readonly Lazy<IIdentityDescriptor> _descriptor;

        private readonly Lazy<IEnumerable<IIdentityDescriptor>> _memberOf;

        private readonly Lazy<IEnumerable<IIdentityDescriptor>> _members;

        internal TeamFoundationIdentityProxy(Tfs.TeamFoundationIdentity identity)
        {
            _identity = identity;
            _descriptor =
                new Lazy<IIdentityDescriptor>(
                    () =>
                        ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(
                            new IdentityDescriptorProxy(_identity?.Descriptor)));

            _memberOf =
                new Lazy<IEnumerable<IIdentityDescriptor>>(
                    () =>
                        _identity?.MemberOf.Select(
                            item =>
                                ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(
                                    new IdentityDescriptorProxy(item))) ?? Enumerable.Empty<IIdentityDescriptor>());

            _members =
                new Lazy<IEnumerable<IIdentityDescriptor>>(
                    () =>
                        _identity?.Members.Select(
                            item =>
                                ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(
                                    new IdentityDescriptorProxy(item))) ?? Enumerable.Empty<IIdentityDescriptor>());
        }

        public IIdentityDescriptor Descriptor => _descriptor.Value;

        public string DisplayName => _identity?.DisplayName;

        public bool IsActive => _identity?.IsActive ?? false;

        public bool IsContainer => _identity?.IsContainer ?? false;

        public IEnumerable<IIdentityDescriptor> MemberOf => _memberOf.Value;

        public IEnumerable<IIdentityDescriptor> Members => _members.Value;

        public Guid TeamFoundationId => _identity?.TeamFoundationId ?? Guid.Empty;

        public string UniqueName => _identity?.UniqueName;

        public int UniqueUserId => _identity?.UniqueUserId ?? -1;
    }
}

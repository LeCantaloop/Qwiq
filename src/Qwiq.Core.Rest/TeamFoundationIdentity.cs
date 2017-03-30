using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;
using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.Qwiq.Rest
{
    public class TeamFoundationIdentity : ITeamFoundationIdentity
    {
        private readonly Lazy<IIdentityDescriptor> _descriptor;

        private readonly Lazy<IEnumerable<IIdentityDescriptor>> _memberOf;

        private readonly Lazy<IEnumerable<IIdentityDescriptor>> _members;

        public TeamFoundationIdentity(Identity identity)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));
            DisplayName = identity.DisplayName;
            IsActive = identity.IsActive;
            IsContainer = identity.IsContainer;
            TeamFoundationId = identity.Id;
            UniqueUserId = identity.UniqueUserId;

            _descriptor = new Lazy<IIdentityDescriptor>(()=>  ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(new IdentityDescriptor(identity.Descriptor)));
            _memberOf =
                new Lazy<IEnumerable<IIdentityDescriptor>>(
                    () =>
                        identity.MemberOf.Select(
                            item =>
                                ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(
                                    new IdentityDescriptor(item))) ?? Enumerable.Empty<IIdentityDescriptor>());

            _members =
                new Lazy<IEnumerable<IIdentityDescriptor>>(
                    () =>
                        identity.Members.Select(
                            item =>
                                ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(
                                    new IdentityDescriptor(item))) ?? Enumerable.Empty<IIdentityDescriptor>());

        }

        public IIdentityDescriptor Descriptor => _descriptor.Value;

        public string DisplayName { get; }

        public bool IsActive { get; }

        public bool IsContainer { get; }

        public IEnumerable<IIdentityDescriptor> MemberOf => _memberOf.Value;

        public IEnumerable<IIdentityDescriptor> Members => _members.Value;

        public Guid TeamFoundationId { get; }

        public string UniqueName { get; }

        public int UniqueUserId { get; }
    }
}

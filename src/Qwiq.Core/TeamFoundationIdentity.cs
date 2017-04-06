using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public abstract class TeamFoundationIdentity : ITeamFoundationIdentity, IEquatable<ITeamFoundationIdentity>
    {
        private string _uniqueName;

        protected internal TeamFoundationIdentity(
            bool isActive,
            Guid teamFoundationId,
            int uniqueUserId
            )
        {
            IsActive = isActive;
            TeamFoundationId = teamFoundationId;
            UniqueUserId = uniqueUserId;
        }

        public bool Equals(ITeamFoundationIdentity other)
        {
            return TeamFoundationIdentityComparer.Instance.Equals(this, other);
        }

        public abstract IIdentityDescriptor Descriptor { get; }

        public abstract string DisplayName { get; }

        public virtual bool IsActive { get; }

        public virtual bool IsContainer
        {
            get
            {
                var schema = GetAttribute(IdentityAttributeTags.SchemaClassName, null);
                if (!string.IsNullOrEmpty(schema) && string.Equals(
                        schema,
                        IdentityConstants.SchemaClassGroup,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return false;
            }
        }

        public abstract IEnumerable<IIdentityDescriptor> MemberOf { get; }

        public abstract IEnumerable<IIdentityDescriptor> Members { get; }

        public virtual Guid TeamFoundationId { get; }

        public virtual string UniqueName
        {
            get
            {
                if (!string.IsNullOrEmpty(_uniqueName)) return _uniqueName;

                var domain = GetAttribute(IdentityAttributeTags.Domain, string.Empty);
                var account = GetAttribute(IdentityAttributeTags.AccountName, string.Empty);

                if (UniqueUserId == IdentityConstants.ActiveUniqueId)
                {
                    _uniqueName = string.IsNullOrEmpty(domain)
                                      ? account
                                      : string.Format(
                                          IdentityConstants.DomainQualifiedAccountNameFormat,
                                          domain,
                                          account);
                }
                else
                {
                    _uniqueName = string.IsNullOrEmpty(domain)
                                      ? $"{account}:{UniqueUserId}"
                                      : $"{string.Format(IdentityConstants.DomainQualifiedAccountNameFormat, domain, account)}:{ UniqueUserId}";
                }

                return _uniqueName;
            }
        }

        public virtual int UniqueUserId { get; }

        public abstract string GetAttribute(string name, string defaultValue);

        public abstract IEnumerable<KeyValuePair<string, object>> GetProperties();

        public abstract object GetProperty(string name);

        public override bool Equals(object obj)
        {
            return Equals(obj as ITeamFoundationIdentity);
        }

        public override int GetHashCode()
        {
            return TeamFoundationIdentityComparer.Instance.GetHashCode(this);
        }

        public override string ToString()
        {
            return $"Identity {TeamFoundationId} (IdentityType: {(Descriptor == null ? string.Empty : Descriptor.IdentityType)}; Identifier: {(Descriptor == null ? string.Empty : Descriptor.Identifier)}; DisplayName: {DisplayName})";
        }
    }
}
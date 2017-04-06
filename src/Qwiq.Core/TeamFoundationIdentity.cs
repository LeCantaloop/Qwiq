using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq
{
    public abstract class TeamFoundationIdentity : ITeamFoundationIdentity, IEquatable<ITeamFoundationIdentity>
    {
        private string _uniqueName;

        public abstract IIdentityDescriptor Descriptor { get; internal set; }

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
                        StringComparison.OrdinalIgnoreCase)) return true;

                return false;
            }
        }

        public abstract IEnumerable<IIdentityDescriptor> MemberOf { get; }

        public abstract IEnumerable<IIdentityDescriptor> Members { get; }

        public virtual Guid TeamFoundationId { get; internal set; }

        public virtual string UniqueName
        {
            get
            {
                if (!string.IsNullOrEmpty(_uniqueName)) return _uniqueName;

                var domain = GetAttribute("Domain", string.Empty);
                var account = GetAttribute("Account", string.Empty);

                if (UniqueUserId == IdentityConstants.ActiveUniqueId) _uniqueName = string.IsNullOrEmpty(domain) ? account : $"{domain}\\{account}";
                else
                    _uniqueName = string.IsNullOrEmpty(domain)
                                      ? $"{account}:{UniqueUserId}"
                                      : $"{domain}\\{account}:{UniqueUserId}";

                return _uniqueName;
            }
        }

        public virtual int UniqueUserId { get; }

        public bool Equals(ITeamFoundationIdentity other)
        {
            return TeamFoundationIdentityComparer.Instance.Equals(this, other);
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as ITeamFoundationIdentity);
        }

        public abstract string GetAttribute(string name, string defaultValue);

        public override int GetHashCode()
        {
            return TeamFoundationIdentityComparer.Instance.GetHashCode(this);
        }

        public abstract IEnumerable<KeyValuePair<string, object>> GetProperties();

        public abstract object GetProperty(string name);
        public override string ToString()
        {
            return UniqueName;
        }
    }
}
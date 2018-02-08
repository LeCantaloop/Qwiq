using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Qwiq
{
    public abstract class TeamFoundationIdentity : ITeamFoundationIdentity, IEquatable<ITeamFoundationIdentity>
    {
        protected internal static readonly IIdentityDescriptor[] ZeroLengthArrayOfIdentityDescriptor = new IIdentityDescriptor[0];
        private string _uniqueName;

        protected internal TeamFoundationIdentity(
            bool isActive,
            Guid teamFoundationId,
            int uniqueUserId
            )
            : this()
        {
            IsActive = isActive;
            TeamFoundationId = teamFoundationId;
            UniqueUserId = uniqueUserId;
        }

        protected internal TeamFoundationIdentity(
            bool isActive,
            Guid teamFoundationId,
            int uniqueUserId,
            IEnumerable<IIdentityDescriptor> memberOf,
            IEnumerable<IIdentityDescriptor> members)
            : this(isActive, teamFoundationId, uniqueUserId)
        {
            MemberOf = memberOf ?? ZeroLengthArrayOfIdentityDescriptor;
            Members = members ?? ZeroLengthArrayOfIdentityDescriptor;
        }

        private TeamFoundationIdentity()
        {
            MemberOf = ZeroLengthArrayOfIdentityDescriptor;
            Members = ZeroLengthArrayOfIdentityDescriptor;
            TeamFoundationId = Guid.Empty;
        }

        public abstract IIdentityDescriptor Descriptor { get; }

        public abstract string DisplayName { get; }

        public bool IsActive { get; }

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

        public IEnumerable<IIdentityDescriptor> MemberOf { get; }

        public IEnumerable<IIdentityDescriptor> Members { get; }

        public Guid TeamFoundationId { get; }

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
                                      ? $"{account}:{UniqueUserId.ToString(CultureInfo.InvariantCulture)}"
                                      : $"{string.Format(IdentityConstants.DomainQualifiedAccountNameFormat, domain, account)}:{UniqueUserId.ToString(CultureInfo.InvariantCulture)}";
                }

                return _uniqueName;
            }
        }

        public int UniqueUserId { get; }

        public bool Equals(ITeamFoundationIdentity other)
        {
            return Comparer.TeamFoundationIdentity.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ITeamFoundationIdentity);
        }

        public abstract string GetAttribute(string name, string defaultValue);

        public override int GetHashCode()
        {
            return Comparer.TeamFoundationIdentity.GetHashCode(this);
        }

        public abstract IEnumerable<KeyValuePair<string, object>> GetProperties();

        public abstract object GetProperty(string name);

        public override string ToString()
        {
            // Call of .ToString to avoid boxing Guid to Object
            // ReSharper disable RedundantToStringCallForValueType
            return $"Identity {TeamFoundationId.ToString()} (IdentityType: {(Descriptor == null ? string.Empty : Descriptor.IdentityType)}; Identifier: {(Descriptor == null ? string.Empty : Descriptor.Identifier)}; DisplayName: {DisplayName})";
            // ReSharper restore RedundantToStringCallForValueType
        }
    }
}
using System;

using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.Qwiq
{
    public class IdentityDescriptor : IIdentityDescriptor, IComparable<IdentityDescriptor>
    {
        protected byte _identityType;
        private string _identifier;

        /// <summary>
        /// </summary>
        /// <param name="identityType"></param>
        /// <param name="identifier"></param>
        /// <example>
        ///     User:
        ///     "Microsoft.IdentityModel.Claims.ClaimsIdentity", "2fa3a376-370f-4226-9fbb-d778e4b5bf74\\ftotten@fabrikam.com"
        ///     Service:
        ///     "Microsoft.TeamFoundation.ServiceIdentity",
        ///     "d9454f90-6587-4699-9357-3e83e331580a:Build:f2200ea9-52cf-4343-8c80-af2cfa409984"
        ///     TFS Identity:
        ///     "Microsoft.TeamFoundation.Identity",
        ///     "S-1-9-1234567890-1234567890-123456789-1234567890-1234567890-1-1234567890-1234567890-1234567890-1234567890"
        /// </example>
        public IdentityDescriptor(string identityType, string identifier)
        {
            IdentityType = identityType;
            Identifier = identifier;
        }

        public string Identifier
        {
            get => _identifier;
            private set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
                if (value.Length > 256) throw new ArgumentOutOfRangeException(nameof(value));
                _identifier = value;
            }
        }

        public string IdentityType
        {
            get => IdentityTypeMapper.Instance.GetTypeNameFromId(_identityType);
            private set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
                if (value.Length > 128) throw new ArgumentOutOfRangeException(nameof(value));
                _identityType = IdentityTypeMapper.Instance.GetTypeIdFromName(value);
            }
        }

        public int CompareTo(IdentityDescriptor other)
        {
            if (this == other)
                return 0;
            if (this == null && other != null)
                return -1;
            if (this != null && other == null)
                return 1;
            int num = 0;
            if (_identityType > other._identityType)
                num = 1;
            else if (_identityType < other._identityType)
                num = -1;
            if (num == 0)
                num = StringComparer.OrdinalIgnoreCase.Compare(Identifier, other.Identifier);
            return num;
        }

        public override string ToString()
        {
            return IdentityTypeMapper.Instance.GetTypeNameFromId(_identityType) + ";" + _identifier;
        }

        public override int GetHashCode()
        {
            return _identityType ^ StringComparer.OrdinalIgnoreCase.GetHashCode(Identifier);
        }
    }
}

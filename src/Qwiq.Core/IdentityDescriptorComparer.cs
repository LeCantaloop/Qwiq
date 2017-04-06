using System;

namespace Microsoft.Qwiq
{
    public class IdentityDescriptorComparer : GenericComparer<IIdentityDescriptor>
    {
        public static IdentityDescriptorComparer Instance => Nested.Instance;

        public override bool Equals(IIdentityDescriptor x, IIdentityDescriptor y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return string.Equals(x.Identifier, y.Identifier, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(x.IdentityType, y.IdentityType, StringComparison.OrdinalIgnoreCase);
        }

        private static readonly System.Security.Cryptography.MD5CryptoServiceProvider Md5Provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
        // the database is usually set to Latin1_General_CI_AS which is codepage 1252
        private static readonly System.Text.Encoding Encoding = System.Text.Encoding.GetEncoding(1252);

        private static int ComputeStringHash(string sourceString, int modulo = 0)
        {
            var md5Bytes = Md5Provider.ComputeHash(Encoding.GetBytes(sourceString));
            var result = BitConverter.ToInt32(new[] { md5Bytes[15], md5Bytes[14], md5Bytes[13], md5Bytes[12] }, 0);
            return modulo == 0
                       ? result
                       : Math.Abs(result) % modulo;
        }

        public override int GetHashCode(IIdentityDescriptor obj)
        {
            unchecked
            {
                var hash = 27;

                hash = (13 * hash) ^ ComputeStringHash(obj.Identifier);
                hash = (13 * hash) ^ ComputeStringHash(obj.IdentityType);

                return hash;
            }
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class Nested
            // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly IdentityDescriptorComparer Instance = new IdentityDescriptorComparer();
            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}
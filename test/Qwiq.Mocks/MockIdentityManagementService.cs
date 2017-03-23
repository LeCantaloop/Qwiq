using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Mocks
{
    public class MockIdentityManagementService : IIdentityManagementService
    {
        public static readonly ITeamFoundationIdentity Danj = new MockTeamFoundationIdentity("Dan Jump", "danj")
        {
            TeamFoundationId = Guid.Parse("b7de08a6-8417-491b-be62-85945a538f46")
        };
        public static readonly ITeamFoundationIdentity Adamb = new MockTeamFoundationIdentity("Adam Barr", "adamb")
        {
            TeamFoundationId = Guid.Parse("7846c22f-d3d8-4e02-8b62-d055d0284783")
        };
        public static readonly ITeamFoundationIdentity Chrisj = new MockTeamFoundationIdentity("Chris Johnson", "chrisj")
        {
            TeamFoundationId = Guid.Parse("f92c1baa-0038-4247-be68-12043fcc34e3"),
            IsActive = false
        };
        public static readonly ITeamFoundationIdentity Chrisjoh = new MockTeamFoundationIdentity("Chris Johnson (FINANCE)", "chrisjoh")
        {
            TeamFoundationId = Guid.Parse("41e97533-89f7-45d7-8246-eaa449b5651d")
        };
        public static readonly ITeamFoundationIdentity Chrisjohn = new MockTeamFoundationIdentity("Chris F. Johnson","chrisjohn")
        {
            TeamFoundationId = Guid.Parse("b3da460c-6191-4725-b08d-52bba48a574f")
        };
        public static readonly ITeamFoundationIdentity Chrisjohns = new MockTeamFoundationIdentity("Chris Johnson <chrisjohns@contoso.com>", "chrisjohns")
        {
            TeamFoundationId = Guid.Parse("67b42b6c-6bd8-40e2-a622-fe69eacd3d47")
        };

        private readonly IDictionary<string, ITeamFoundationIdentity[]> _accountNameMappings;
        private readonly IDictionary<IIdentityDescriptor, ITeamFoundationIdentity> _descriptorMappings;

        /// <summary>
        /// Initializes a new instance of the IMS with Contoso users (danj, adamb, chrisj, chrisjoh, chrisjohn, chrisjohns)
        /// </summary>
        public MockIdentityManagementService()
            : this(new[]
                       {
                           Danj,
                           Adamb,
                           Chrisj,
                           Chrisjoh,
                           Chrisjohn,
                           Chrisjohns,
                        })
        {
        }

        public MockIdentityManagementService(IEnumerable<ITeamFoundationIdentity> identities)
            : this(identities.ToDictionary(k => k.GetUserAlias(), e => e, StringComparer.OrdinalIgnoreCase))
        {
        }

        private class IIdentityDescriptorComparer : IComparer, IEqualityComparer, IComparer<IIdentityDescriptor>, IEqualityComparer<IIdentityDescriptor>
        {
            public int Compare(object x, object y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                var da = x as IIdentityDescriptor;
                if (da != null)
                {
                    var db = y as IIdentityDescriptor;
                    if (db != null)
                    {
                        return Compare(da, db);
                    }
                }

                var ia = x as IComparable;
                if (ia != null)
                {
                    return ia.CompareTo(y);
                }

                throw new ArgumentException("Argument must implement IComparable");
            }

            public new bool Equals(object x, object y)
            {
                if (x == y) return true;
                if (x == null || y == null) return false;

                var da = x as IIdentityDescriptor;
                if (da != null)
                {
                    var db = y as IIdentityDescriptor;
                    if (db != null)
                    {
                        return Equals(da, db);
                    }
                }

                return x.Equals(y);
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

            public int GetHashCode(object obj)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException(nameof(obj));
                }
                var s = obj as IIdentityDescriptor;
                return s != null
                    ? GetHashCode(s)
                    : obj.GetHashCode();
            }

            public int Compare(IIdentityDescriptor x, IIdentityDescriptor y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                var xt = x.IdentityType;
                var xi = x.Identifier;
                var yt = y.IdentityType;
                var yi = y.Identifier;

                return StringComparer.OrdinalIgnoreCase.Compare(xt + xi, yt + yi);
            }

            public bool Equals(IIdentityDescriptor x, IIdentityDescriptor y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;

                var xt = x.IdentityType;
                var xi = x.Identifier;
                var yt = y.IdentityType;
                var yi = y.Identifier;

                return StringComparer.OrdinalIgnoreCase.Equals(xt + xi, yt + yi);
            }

            public int GetHashCode(IIdentityDescriptor obj)
            {
                var hash = 17;
                if (!string.IsNullOrEmpty(obj.IdentityType)) hash = hash * 23 + ComputeStringHash(obj.IdentityType);
                if (!string.IsNullOrEmpty(obj.Identifier)) hash = hash * 23 + ComputeStringHash(obj.Identifier);

                return hash;
            }
        }

        /// <summary>
        /// Creates a new instance of the IMS
        /// </summary>
        /// <param name="accountNameMappings">
        /// Collection of alias to <see cref="ITeamFoundationIdentity"/>.
        /// </param>
        public MockIdentityManagementService(IDictionary<string, ITeamFoundationIdentity> accountNameMappings)
        {
            _accountNameMappings = new Dictionary<string, ITeamFoundationIdentity[]>(StringComparer.OrdinalIgnoreCase);
            _descriptorMappings = new Dictionary<IIdentityDescriptor, ITeamFoundationIdentity>(new IIdentityDescriptorComparer());

            foreach (var account in accountNameMappings)
            {
                var success = _accountNameMappings.TryAdd(account.Key, new[] { account.Value });
                if (!success) Trace.TraceWarning("Account {0} not added; account already exists", account.Key);
            }

            foreach (var accounts in _accountNameMappings.Values)
            {
                foreach (var account in accounts)
                {
                    var success = _descriptorMappings.TryAdd(account.Descriptor, account);
                    if (!success) Trace.TraceWarning("Account {0} not added; account already exists", account.Descriptor.Identifier);
                }
            }
        }

        /// <summary>
        /// Creates a new instance of the IMS
        /// </summary>
        /// <param name="userMappings">
        /// Collection of alias and display names for which to initialize the IMS
        /// </param>
        public MockIdentityManagementService(IDictionary<string, string> userMappings)
            : this(userMappings.ToDictionary(kvp => kvp.Key, kvp => new MockTeamFoundationIdentity(kvp.Value, kvp.Key + "@domain.local") as ITeamFoundationIdentity))
        {
        }

        public MockIdentityManagementService(IDictionary<string, IEnumerable<ITeamFoundationIdentity>> accountMappings)
        {
            _accountNameMappings = accountMappings?.ToDictionary(k => k.Key, e => e.Value.ToArray())
                                    ?? new Dictionary<string, ITeamFoundationIdentity[]>(StringComparer.OrdinalIgnoreCase);
            _descriptorMappings = new Dictionary<IIdentityDescriptor, ITeamFoundationIdentity>(new IIdentityDescriptorComparer());

            foreach (var accounts in _accountNameMappings.Values)
            {
                foreach (var account in accounts)
                {
                    _descriptorMappings.Add(account.Descriptor, account);
                }
            }
        }


        public IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier)
        {
            return new MockIdentityDescriptor(identityType, identifier);
        }

        /// <summary>
        /// Read identities for given descriptors.
        /// </summary>
        /// <param name="descriptors">Collection of <see cref="IIdentityDescriptor"/></param>
        /// <returns>
        /// An array of <see cref="ITeamFoundationIdentity"/>, corresponding 1 to 1 with input descriptor array.
        /// </returns>
        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(ICollection<IIdentityDescriptor> descriptors)
        {
            foreach (var descriptor in descriptors)
            {
                ITeamFoundationIdentity identity;
                _descriptorMappings.TryGetValue(descriptor, out identity);
                yield return identity;
            }
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(IdentitySearchFactor searchFactor, ICollection<string> searchFactorValues)
        {
            switch (searchFactor)
            {
                // Alternate login username
                case IdentitySearchFactor.Alias:
                    foreach (var value in searchFactorValues)
                    {
                        if (_accountNameMappings.ContainsKey(value))
                        {
                            yield return
                                new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(
                                    value,
                                    _accountNameMappings[value]);
                        }
                        else
                        {
                            yield return new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(value, new ITeamFoundationIdentity[0]);
                        }
                    }
                    break;
                // Windows NT account name: domain\alias.
                case IdentitySearchFactor.AccountName:
                    foreach (var value in searchFactorValues)
                    {
                        yield return new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(
                            value,
                            Locate(identity => identity.UniqueName.StartsWith(value, StringComparison.OrdinalIgnoreCase)));
                    }
                    break;
                // Display name
                case IdentitySearchFactor.DisplayName:
                    foreach (var value in searchFactorValues)
                    {
                        yield return
                            new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(
                                value,
                                Locate(identity =>
                                value.Equals(identity.DisplayName, StringComparison.OrdinalIgnoreCase) ||
                                identity.DisplayName.StartsWith(value, StringComparison.OrdinalIgnoreCase)));
                    }
                    break;
                case IdentitySearchFactor.AdministratorsGroup:
                case IdentitySearchFactor.Identifier:
                case IdentitySearchFactor.MailAddress:
                case IdentitySearchFactor.General:
                    throw new NotSupportedException();
            }
        }

        private IEnumerable<ITeamFoundationIdentity> Locate(Func<ITeamFoundationIdentity, bool> predicate)
        {
            return _accountNameMappings
                        .Values
                        .SelectMany(a => a, (a, i) => new { a, i })
                        .Where(@t => predicate(@t.i))
                        .Select(@t => @t.i)
                        .ToArray();
        }
    }
}


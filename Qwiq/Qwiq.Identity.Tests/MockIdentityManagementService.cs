using System;
using System.Collections.Generic;
using Microsoft.IE.Qwiq;

namespace Qwiq.Identity.Tests
{
    public class MockIdentityManagementService : IIdentityManagementService
    {
        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(IEnumerable<IIdentityDescriptor> descriptors)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(IdentitySearchFactor searchFactor, string[] searchFactorValues)
        {
            throw new NotImplementedException();
        }

        public IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier)
        {
            throw new NotImplementedException();
        }
    }
}
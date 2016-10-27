using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.Qwiq.Mocks
{
    public class MockTfsTeamProjectCollection : ITfsTeamProjectCollection
    {
        public MockTfsTeamProjectCollection()
            : this(new MockIdentityManagementService())
        {
        }

        public MockTfsTeamProjectCollection(IIdentityManagementService identityManagementService)
        {
            IdentityManagementService = identityManagementService;
        }

        public ICommonStructureService CommonStructureService { get; set; }

        public IIdentityManagementService IdentityManagementService { get; set; }
    }
}

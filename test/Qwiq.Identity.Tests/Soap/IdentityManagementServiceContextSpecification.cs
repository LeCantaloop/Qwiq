using System.Collections.Generic;

using Qwiq.Identity.Mocks;
using Qwiq.Tests.Common;
using Microsoft.TeamFoundation.Framework.Client;

namespace Qwiq.Identity.Soap
{
    public abstract class IdentityManagementServiceContextSpecification<T> : ContextSpecification
    {
        private IIdentityManagementService2 _identityManagementService2;
        protected IIdentityManagementService Service;

        protected IEnumerable<T> Actual;

        public override void Given()
        {
            _identityManagementService2 = new MockIdentityManagementService2();
            Service = new IdentityManagementService(_identityManagementService2);
        }
    }
}
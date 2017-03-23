using System.Collections.Generic;
using System.Linq;
using Microsoft.Qwiq.Core.Tests.Mocks;
using Microsoft.Qwiq.Proxies.Soap;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Microsoft.Qwiq.Core.Tests
{
    public abstract class IdentityManagementServiceProxyTests<T> : ContextSpecification
    {
        private IIdentityManagementService2 _identityManagementService2;
        protected IIdentityManagementService Service;

        protected IEnumerable<T> Actual;

        public override void Given()
        {
            _identityManagementService2 = new MockIdentityManagementService2();
            Service = new IdentityManagementServiceProxy(_identityManagementService2);
        }
    }

    [TestClass]
    public class when_ReadIdentities_using_a_search_factor_for_an_identity_that_doesnt_exist : IdentityManagementServiceProxyTests<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>>
    {
        public override void When()
        {
            Actual = Service.ReadIdentities(IdentitySearchFactor.AccountName, new[] {"I Do Not Exist"});
        }

        [TestMethod]
        public void a_null_is_returned_instead_of_a_TeamFoundationIdentity()
        {
            Actual.Single().Value.Single().ShouldBeNull();
        }
    }

    [TestClass]
    public class when_ReadIdentities_using_an_IIdentityDescriptor_for_an_identity_that_doesnt_exist :
        IdentityManagementServiceProxyTests<ITeamFoundationIdentity>
    {
        public override void When()
        {
            Actual = Service.ReadIdentities(new [] { new MockIdentityDescriptor() });
        }

        [TestMethod]
        public void a_null_is_returned_instead_of_a_TeamFoundationIdentity()
        {
            Actual.Single().ShouldBeNull();
        }
    }
}


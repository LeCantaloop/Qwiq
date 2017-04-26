using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Identity.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Identity.Soap
{
    [TestClass]
    public class when_ReadIdentities_using_a_search_factor_for_an_identity_that_doesnt_exist : IdentityManagementServiceContextSpecification<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>>
    {
        public override void When()
        {
            Actual = Service.ReadIdentities(IdentitySearchFactor.AccountName, new[] {"I Do Not Exist"});
        }

        [TestMethod]
        [TestCategory("SOAP")]
        public void a_null_is_returned_instead_of_a_TeamFoundationIdentity()
        {
            Actual.Single().Value.Single().ShouldBeNull();
        }
    }

    [TestClass]
    public class when_ReadIdentities_using_an_IIdentityDescriptor_for_an_identity_that_doesnt_exist :
        IdentityManagementServiceContextSpecification<ITeamFoundationIdentity>
    {
        public override void When()
        {
            Actual = Service.ReadIdentities(new [] { new MockIdentityDescriptor() });
        }

        [TestMethod]
        [TestCategory("SOAP")]
        public void a_null_is_returned_instead_of_a_TeamFoundationIdentity()
        {
            Actual.Single().ShouldBeNull();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Qwiq.Identity.Soap
{
    [TestClass]
    public class Given_an_Account_with_Group_Membership : SoapIdentityManagementServiceContextSpecification
    {
        private string _input;
        private ITeamFoundationIdentity _result;

        public override void Given()
        {
            base.Given();

            _input = "rimuri@microsoft.com";
        }

        public override void When()
        {
            _result = Instance.ReadIdentity(IdentitySearchFactor.AccountName, _input, MembershipQuery.Expanded);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Identity_Contains_MemberOf()
        {
            _result.MemberOf.Any().ShouldBeTrue();
        }
    }
}

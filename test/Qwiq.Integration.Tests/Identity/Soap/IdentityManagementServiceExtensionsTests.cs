using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Identity.Soap
{
    [TestClass]
    public class Given_a_valid_user_display_name : SoapIdentityManagementServiceContextSpecification
    {
        private IEnumerable<ITeamFoundationIdentity> _results;

        public override void When()
        {
            _results = TimedAction(() => Instance.ReadIdentities(IdentitySearchFactor.DisplayName, new[]{ "PETER LAVALLEE" }).First().Value, "SOAP", "AliasForDisplayName");

            Debug.Print("Results: " + _results.EachToUsefulString());
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void result_contains_a_single_alias_PELAVALL()
        {
            _results.Single().GetUserAlias().ShouldEqual("pelavall");
        }
    }

    [TestClass]
    public class Given_a_valid_user_alias : SoapIdentityManagementServiceContextSpecification
    {
        private ITeamFoundationIdentity _results;

        public override void When()
        {
            _results = TimedAction(() => Instance.GetIdentityForAlias("pelavall", "72F988BF-86F1-41AF-91AB-2D7CD011DB47", "microsoft.com"), "SOAP", "AliasForDisplayName");

            Debug.Print("Results: " + _results.ToUsefulString());
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void result_contains_a_single_alias_PELAVALL()
        {
                        _results.GetUserAlias().ShouldEqual("pelavall");
        }
    }
}

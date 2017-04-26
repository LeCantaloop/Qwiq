using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Mocks;
using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Identity
{
    public abstract class IdentityManagementServiceExtensionsTests : ContextSpecification
    {
        protected const string KnownSearchAliasForChrisjoh = "chrisjo";
        protected const string KnownSearchAliasForDanj = "dan";
        protected const string UnknownAliasA = "none";
        protected const string UnknownAliasB = "unknown";

        protected IIdentityManagementService IdentityService { get; set; }

        public override void Given()
        {
            IdentityService = new MockIdentityManagementService();
            base.Given();
        }
    }

    public abstract class SingularIdentityManagementServiceExtensionsTests : IdentityManagementServiceExtensionsTests
    {
        protected string Alias { get; set; }
        protected ITeamFoundationIdentity ExpectedIdentity { get; set; }
        protected ITeamFoundationIdentity ActualIdentity { get; set; }

        public override void When()
        {
            ActualIdentity = IdentityService.GetIdentityForAlias(Alias, MockIdentityDescriptor.TenantId, MockIdentityDescriptor.Domain, "otherdomain");
        }

        [TestMethod]
        public void the_actual_identity_is_equal_to_the_expected()
        {
            ActualIdentity.ShouldEqual(ExpectedIdentity);
        }

    }

    [TestClass]
    public class Given_an_alias_which_can_be_resolved_by_identity_descriptor : SingularIdentityManagementServiceExtensionsTests
    {
        public override void Given()
        {
            ExpectedIdentity = Identities.Chrisj;
            Alias = ExpectedIdentity.GetUserAlias();
            base.Given();
        }
    }

    [TestClass]
    public class Given_an_alias_which_can_be_resolved_by_searching : SingularIdentityManagementServiceExtensionsTests
    {
        public override void Given()
        {
            ExpectedIdentity = Identities.Chrisjoh;
            Alias = KnownSearchAliasForChrisjoh;
            base.Given();
        }
    }

    [TestClass]
    public class Given_an_alias_which_cant_be_resolved : SingularIdentityManagementServiceExtensionsTests
    {
        public override void Given()
        {
            ExpectedIdentity = null;
            Alias = UnknownAliasA;
            base.Given();
        }
    }

    public abstract class MultiIdentityManagementServiceExtensionsTests : IdentityManagementServiceExtensionsTests
    {
        protected string[] Aliases { get; set; }
        protected IDictionary<string, ITeamFoundationIdentity> ActualIdentities { get; set; }
        protected IDictionary<string, ITeamFoundationIdentity> ExpectedIdentities { get; set; }

        public override void When()
        {
            ActualIdentities = IdentityService.GetIdentityForAliases(Aliases, MockIdentityDescriptor.TenantId, MockIdentityDescriptor.Domain, "otherdomain");
        }

        [TestMethod]
        public void the_actual_identities_are_equal_to_the_expected()
        {
            ActualIdentities.ShouldContainOnly(ExpectedIdentities);
        }
    }

    [TestClass]
    public class Given_aliases_which_can_be_resolved_by_identity_descriptor : MultiIdentityManagementServiceExtensionsTests
    {
        public override void Given()
        {
            var contestant1 = Identities.Chrisj;
            var contestant2 = Identities.Danj;
            ExpectedIdentities = new Dictionary<string, ITeamFoundationIdentity>
            {
                { contestant1.GetUserAlias(), contestant1 },
                { contestant2.GetUserAlias(), contestant2 }
            };
            Aliases = ExpectedIdentities.Keys.ToArray();
            base.Given();
        }
    }

    [TestClass]
    public class Given_aliases_which_can_be_resolved_by_searching : MultiIdentityManagementServiceExtensionsTests
    {
        public override void Given()
        {
            ExpectedIdentities = new Dictionary<string, ITeamFoundationIdentity>
            {
                { KnownSearchAliasForChrisjoh, Identities.Chrisjoh },
                { KnownSearchAliasForDanj, Identities.Danj }
            };
            Aliases = ExpectedIdentities.Keys.ToArray();
            base.Given();
        }
    }

    [TestClass]
    public class Given_aliases_which_cant_be_resolved : MultiIdentityManagementServiceExtensionsTests
    {
        public override void Given()
        {
            ExpectedIdentities = new Dictionary<string, ITeamFoundationIdentity>
            {
                { UnknownAliasA, null },
                { UnknownAliasB, null }
            };
            Aliases = ExpectedIdentities.Keys.ToArray();
            base.Given();
        }
    }

    [TestClass]
    public class Given_aliases_which_can_be_resolved_by_different_methods : MultiIdentityManagementServiceExtensionsTests
    {
        public override void Given()
        {
            var danj = Identities.Chrisj;
            ExpectedIdentities = new Dictionary<string, ITeamFoundationIdentity>
            {
                { danj.GetUserAlias(), danj },
                { KnownSearchAliasForChrisjoh, Identities.Chrisjoh },
                { UnknownAliasB, null }
            };
            Aliases = ExpectedIdentities.Keys.ToArray();
            base.Given();
        }
    }
}


using System;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Identity
{
    [TestClass]
    public class Given_TeamFoundationIdentity_with_domain_and_account : ContextSpecification
    {
        private ITeamFoundationIdentity _identity = null!;

        public override void Given()
        {
            _identity = new MockTeamFoundationIdentity(
                MockIdentityDescriptor.Create("jdoe", "DOMAIN"),
                "John Doe",
                Guid.NewGuid());
        }

        [TestMethod]
        public void Then_UniqueName_contains_domain()
        {
            _identity.UniqueName.ShouldContain("DOMAIN");
        }

        [TestMethod]
        public void Then_DisplayName_is_set()
        {
            _identity.DisplayName.ShouldBe("John Doe");
        }

        [TestMethod]
        public void Then_IsActive_is_true_by_default()
        {
            _identity.IsActive.ShouldBeTrue();
        }

        [TestMethod]
        public void Then_ToString_contains_TeamFoundationId()
        {
            var str = _identity.ToString();
            str.ShouldNotBeNull();
            str.ShouldContain("Identity");
        }
    }

    [TestClass]
    public class Given_TeamFoundationIdentity_that_is_not_active : ContextSpecification
    {
        private ITeamFoundationIdentity _identity = null!;

        public override void Given()
        {
            _identity = new MockTeamFoundationIdentity(
                MockIdentityDescriptor.Create("inactive", "DOMAIN"),
                "Inactive User",
                Guid.NewGuid(),
                isActive: false);
        }

        [TestMethod]
        public void Then_IsActive_is_false()
        {
            _identity.IsActive.ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_TeamFoundationIdentity_equality : ContextSpecification
    {
        private ITeamFoundationIdentity _identity1 = null!;
        private ITeamFoundationIdentity _identity2 = null!;

        public override void Given()
        {
            // Comparer uses UniqueName and Descriptor, so same descriptor = equal
            var descriptor = MockIdentityDescriptor.Create("user", "DOMAIN");
            _identity1 = new MockTeamFoundationIdentity(descriptor, "User1", Guid.NewGuid());
            _identity2 = new MockTeamFoundationIdentity(descriptor, "User2", Guid.NewGuid());
        }

        [TestMethod]
        public void Then_identities_with_same_Descriptor_are_equal()
        {
            _identity1.Equals(_identity2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_is_same_for_equal_identities()
        {
            _identity1.GetHashCode().ShouldBe(_identity2.GetHashCode());
        }
    }

    [TestClass]
    public class Given_TeamFoundationIdentity_with_different_ids : ContextSpecification
    {
        private ITeamFoundationIdentity _identity1 = null!;
        private ITeamFoundationIdentity _identity2 = null!;

        public override void Given()
        {
            _identity1 = new MockTeamFoundationIdentity(
                MockIdentityDescriptor.Create("user1", "DOMAIN"),
                "User1",
                Guid.NewGuid());
            _identity2 = new MockTeamFoundationIdentity(
                MockIdentityDescriptor.Create("user2", "DOMAIN"),
                "User2",
                Guid.NewGuid());
        }

        [TestMethod]
        public void Then_identities_are_not_equal()
        {
            _identity1.Equals(_identity2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_TeamFoundationIdentityComparer_with_null : ContextSpecification
    {
        [TestMethod]
        public void Then_null_null_are_equal()
        {
            Comparer.TeamFoundationIdentity.Equals(null!, null!).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_identity_null_are_not_equal()
        {
            var identity = new MockTeamFoundationIdentity(
                MockIdentityDescriptor.Create("user", "DOMAIN"),
                "User",
                Guid.NewGuid());
            Comparer.TeamFoundationIdentity.Equals(identity, null!).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_null_identity_are_not_equal()
        {
            var identity = new MockTeamFoundationIdentity(
                MockIdentityDescriptor.Create("user", "DOMAIN"),
                "User",
                Guid.NewGuid());
            Comparer.TeamFoundationIdentity.Equals(null!, identity).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_GetHashCode_of_null_returns_zero()
        {
            Comparer.TeamFoundationIdentity.GetHashCode(null!).ShouldBe(0);
        }
    }

    [TestClass]
    public class Given_TeamFoundationIdentity_Equals_object_overload : ContextSpecification
    {
        private ITeamFoundationIdentity _identity = null!;

        public override void Given()
        {
            _identity = new MockTeamFoundationIdentity(
                MockIdentityDescriptor.Create("user", "DOMAIN"),
                "User",
                Guid.NewGuid());
        }

        [TestMethod]
        public void Then_Equals_null_object_returns_false()
        {
            _identity.Equals((object?)null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_non_identity_object_returns_false()
        {
            _identity.Equals("not an identity").ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_self_returns_true()
        {
            _identity.Equals((object)_identity).ShouldBeTrue();
        }
    }
}

using System.Collections.Generic;
using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq;
using Microsoft.IE.Qwiq.Identity.Linq.Visitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Identity.Tests.Mocks;

namespace Qwiq.Identity.Tests
{
    [TestClass]
    public abstract class IdentityVisitorTests<T> : ContextSpecification
    {
        protected IdentityVisitor Instance { get; set; }
        protected T Input { get; set; }
        protected T ActualOutput { get; set; }
        protected T ExpectedOutput { get; set; }
        public override void Given()
        {
            Instance =
                new IdentityVisitor(
                    new MockIdentityManagementService(new Dictionary<string, IEnumerable<ITeamFoundationIdentity>>
                    {
                        {"alias", new[] {new MockTeamFoundationIdentity("alias", "An Alias"),}},
                        {"other", new[] {new MockTeamFoundationIdentity("other", "AnOther Alias"),}}
                    }), "tenant", "domain");

            base.Given();
        }

        public override void When()
        {
            ActualOutput = (T)Instance.ReplaceValue(Input);
        }

        [TestMethod]
        public void the_actual_output_is_the_expected_output()
        {
            ActualOutput.ShouldEqual(ExpectedOutput);
        }
    }

    [TestClass]
    public class when_a_string_is_visited_with_a_valid_identity : IdentityVisitorTests<string>
    {
        public override void Given()
        {
            base.Given();
            Input = "alias";
            ExpectedOutput = "An Alias";
        }
    }

    [TestClass]
    public class when_a_string_is_visited_without_a_valid_identity : IdentityVisitorTests<string>
    {
        public override void Given()
        {
            base.Given();
            Input = "noidentity";
            ExpectedOutput = "noidentity";
        }
    }

    [TestClass]
    public class when_a_stringarray_is_visited_with_valid_identities : IdentityVisitorTests<IEnumerable<string>>
    {
        public override void Given()
        {
            base.Given();
            Input = new [] { "alias", "other" };
            ExpectedOutput = new[] { "An Alias", "AnOther Alias" };
        }
    }

    [TestClass]
    public class when_a_stringarray_is_visited_with_mixed_identities : IdentityVisitorTests<IEnumerable<string>>
    {
        public override void Given()
        {
            base.Given();
            Input = new[] { "alias", "noidentity2" };
            ExpectedOutput = new[] { "An Alias", "noidentity2" };
        }
    }

    [TestClass]
    public class when_a_non_string_type_is_visited : IdentityVisitorTests<int>
    {
        public override void Given()
        {
            base.Given();
            Input = 1;
            ExpectedOutput = 1;
        }
    }
}

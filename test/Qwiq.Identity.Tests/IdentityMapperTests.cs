using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Qwiq.Mocks;
using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Identity
{
    [TestClass]
    public abstract class IdentityMapperTests<T> : ContextSpecification
    {
        protected IdentityAliasValueConverter Instance { get; set; }
        protected T Input { get; set; }
        protected T ActualOutput { get; set; }
        protected T ExpectedOutput { get; set; }
        public override void Given()
        {
            Instance =
                    new IdentityAliasValueConverter(
                                       new MockIdentityManagementService(new Dictionary<string, ITeamFoundationIdentity>
                                                                             {
                                                                                 {"alias", new MockTeamFoundationIdentity(MockIdentityDescriptor.Create("alias", "domain", "tenant"), "An Alias", Guid.Empty)},
                                                                                 {"other", new MockTeamFoundationIdentity(MockIdentityDescriptor.Create("other", "domain", "tenant"), "AnOther Alias", Guid.Empty)}
                                                                             }), "tenant", "domain");

            base.Given();
        }

        public override void When()
        {
            var result = Instance.Map(Input);

            Debug.Print("Result: " + result.ToUsefulString());

            ActualOutput = (T)result;
        }

        [TestMethod]
        public void the_actual_output_is_the_expected_output()
        {
            ActualOutput.ShouldEqual(ExpectedOutput);
        }
    }

    [TestClass]
    public class when_a_string_is_mapped_with_a_valid_identity : IdentityMapperTests<string>
    {
        public override void Given()
        {
            base.Given();
            Input = "alias";
            ExpectedOutput = "alias@domain";
        }
    }

    [TestClass]
    public class when_a_string_is_mapped_without_a_valid_identity : IdentityMapperTests<string>
    {
        public override void Given()
        {
            base.Given();
            Input = "noidentity";
            ExpectedOutput = "noidentity";
        }
    }

    [TestClass]
    public class when_a_stringarray_is_mapped_with_valid_identities : IdentityMapperTests<IEnumerable<string>>
    {
        public override void Given()
        {
            base.Given();
            Input = new[] { "alias", "other" };
            ExpectedOutput = new[] { "alias@domain", "other@domain" };
        }
    }

    [TestClass]
    public class when_a_stringarray_is_mapped_with_mixed_identities : IdentityMapperTests<IEnumerable<string>>
    {
        public override void Given()
        {
            base.Given();
            Input = new[] { "alias", "noidentity2" };
            ExpectedOutput = new[] { "alias@domain", "noidentity2" };
        }
    }

    [TestClass]
    public class when_a_non_string_type_is_mapped : IdentityMapperTests<int>
    {
        public override void Given()
        {
            base.Given();
            Input = 1;
            ExpectedOutput = 1;
        }
    }
}


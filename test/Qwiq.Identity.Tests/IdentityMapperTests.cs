using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.Identity
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
#pragma warning disable IDE0019 // Use pattern matching
            var stringValue = Input as string;
            var stringArray = Input as IEnumerable<string>;
#pragma warning restore IDE0019 // Use pattern matching

            if (stringValue != null)
            {
                ActualOutput = (T)Instance.Map(stringValue);
            }
            else if (stringArray != null)
            {
                ActualOutput = (T)Instance.Map(stringArray).Values;
            }
            else
            {
                ActualOutput = Input;
            }

            Debug.Print("Result: " + ActualOutput.ToUsefulString());
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

        public override void When()
        {
            ActualOutput = Instance.Map(Input).Values.Select(s => s.ToString());
            Debug.Print("Result: " + ActualOutput.ToUsefulString());
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

        public override void When()
        {
            ActualOutput = Instance.Map(Input).Values.Select(s => s.ToString());
            Debug.Print("Result: " + ActualOutput.ToUsefulString());
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


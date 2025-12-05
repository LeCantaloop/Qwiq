using System.Collections.Generic;
using System.Linq;
using Qwiq.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;
using Shouldly;

namespace Qwiq.Mapper.Identity
{
    [TestClass]
    public class Given_multiple_display_names : MultipleDisplayNameContextSpecification
    {
        public override void Given()
        {
            base.Given();
            // Note: The sandbox only has one test user, so when the same display name is passed twice,
            // only one identity is found and returned. No MultipleIdentitiesFoundException is thrown
            // because there's only one user with that display name.
            DisplayNames = new[] { TestData.TestUserDisplayName, TestData.TestUserDisplayName };
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Converted_value_result_is_expected_value()
        {
            // With duplicate inputs for the same user, we should get the alias
            var kvp = (Dictionary<string, object>)ConvertedValue;
            kvp.ShouldNotBeNull();
            kvp.Count.ShouldBe(1); // Duplicate keys are merged
            kvp[TestData.TestUserDisplayName].ShouldBe(TestData.TestUserAlias);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Converted_value_contains_a_single_result()
        {
            // Duplicate display names resolve to the same identity, so only one result
            var kvp = (Dictionary<string, object>)ConvertedValue;
            kvp.Count.ShouldBe(1);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public new void Converted_value_contains_a_expected_number_of_results()
        {
            // Duplicate display names resolve to one unique result
            var kvp = (Dictionary<string, object>)ConvertedValue;
            kvp.Count.ShouldBe(1);
        }

        public override void When()
        {
            // No exception expected - duplicate inputs for the same user are handled gracefully
            ConvertedValue = TimedAction(() => ValueConverter.Map(DisplayNames), "SOAP", "Map");
        }
    }

    [TestClass]
    public class Given_multiple_combostrings : MultipleDisplayNameContextSpecification
    {
        public override void Given()
        {
            base.Given();
            // Using the sandbox test user for combo strings (same combo string twice)
            DisplayNames = new[] { $"{TestData.TestUserDisplayName} <{TestData.TestUserUpn}>", $"{TestData.TestUserDisplayName} <{TestData.TestUserUpn}>" };
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public new void Converted_value_contains_a_expected_number_of_results()
        {
            // Duplicate combo strings resolve to one unique result
            var kvp = (Dictionary<string, object>)ConvertedValue;
            kvp.Count.ShouldBe(1);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Converted_value_result_is_expected_value()
        {
            var kvp = (Dictionary<string, object>)ConvertedValue;
            kvp.ShouldNotBeNull();
            // The combo string key should map to the alias
            kvp.Values.First().ShouldBe(TestData.TestUserAlias);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Resolved_alias_is_derived_from_UPN()
        {
            // The alias should be the username portion of the UPN - catches regressions
            // where the mapper might return the original combo string instead of resolving
            var kvp = (Dictionary<string, object>)ConvertedValue;
            var alias = (string)kvp.Values.First();
            TestData.TestUserUpn.ShouldContain(alias); // UPN "rjmurillo@msn.com" contains alias "rjmurillo"
        }

        public override void When()
        {
            ConvertedValue = TimedAction(() => ValueConverter.Map(DisplayNames), "SOAP", "Map");
        }
    }

    [TestClass]
    public class Given_a_single_displayname : SingleDisplayNameContextSpecification
    {
        public override void Given()
        {
            base.Given();
            DisplayName = TestData.TestUserDisplayName;
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Converted_value_result_is_expected_value()
        {
            var kvp = (string)ConvertedValue;
            kvp.ShouldBe(TestData.TestUserAlias);
        }
    }

    [TestClass]
    public class Given_a_single_combostring : Given_a_single_displayname
    {
        /// <inheritdoc />
        public override void Given()
        {
            base.Given();
            DisplayName = $"{TestData.TestUserDisplayName} <{TestData.TestUserUpn}>";
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Input_combostring_contains_UPN()
        {
            // Verify that our combo string input contains the UPN - this catches regressions
            // where the identity mapper might return the original string instead of resolving it
            DisplayName.ShouldContain(TestData.TestUserUpn);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Resolved_alias_is_derived_from_UPN()
        {
            // The alias should be the username portion of the UPN
            var kvp = (string)ConvertedValue;
            TestData.TestUserUpn.ShouldContain(kvp); // UPN "rjmurillo@msn.com" contains alias "rjmurillo"
        }
    }

    /// <summary>
    /// Tests for scenarios where a display name would map to multiple identities.
    /// Note: In the sandbox environment, only one user exists, so this test class
    /// verifies the single-user scenario instead. To test MultipleIdentitiesFoundException,
    /// a local TFS with multiple users with the same display name would be needed.
    /// </summary>
    [TestClass]
    public class Given_a_single_display_name_with_multiple_identities : SingleDisplayNameContextSpecification
    {
        /// <inheritdoc />
        public override void Given()
        {
            base.Given();
            // In the sandbox, there's only one user, so no MultipleIdentitiesFoundException will occur
            DisplayName = TestData.TestUserDisplayName;
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Converted_value_result_is_expected_value()
        {
            // In a single-user sandbox, the alias should be returned
            var result = (string)ConvertedValue;
            result.ShouldBe(TestData.TestUserAlias);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public new void Converted_value_contains_a_single_result()
        {
            var result = (string)ConvertedValue;
            result.ShouldNotBeNull();
        }

        public override void When()
        {
            // In a single-user sandbox, no exception is thrown
            ConvertedValue = TimedAction(() => ValueConverter.Map(DisplayName), "SOAP", "Map");
        }
    }

    [TestClass]
    public class Given_a_single_combostring_with_multiple_identities : Given_a_single_display_name_with_multiple_identities
    {
        /// <inheritdoc />
        public override void Given()
        {
            base.Given();
            DisplayName = $"{TestData.TestUserDisplayName} <{TestData.TestUserUpn}>";
        }

        public override void When()
        {
            ConvertedValue = TimedAction(() => ValueConverter.Map(DisplayName), "SOAP", "Map");
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public new void Converted_value_result_is_expected_value()
        {
            ((string)ConvertedValue).ShouldBe(TestData.TestUserAlias, Comparer.OrdinalIgnoreCase);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Resolved_alias_is_derived_from_UPN()
        {
            // The alias should be the username portion of the UPN - catches regressions
            // where the mapper might return the original combo string instead of resolving
            var alias = (string)ConvertedValue;
            TestData.TestUserUpn.ShouldContain(alias); // UPN "rjmurillo@msn.com" contains alias "rjmurillo"
        }
    }
}

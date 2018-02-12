using System.Collections.Generic;
using System.Linq;
using Qwiq.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.Mapper.Identity
{
    [TestClass]
    public class Given_multiple_display_names : MultipleDisplayNameContextSpecification
    {
        public override void Given()
        {
            base.Given();
            DisplayNames = new[] { "Peter Lavallee", "Jason Weber" };
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Converted_value_result_is_expected_value()
        {
            ConvertedValue.ShouldBeNull();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [Ignore]
        public void Converted_value_contains_a_single_result()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [Ignore]
        public new void Converted_value_contains_a_expected_number_of_results()
        {
            Assert.Inconclusive();
        }

        public override void When()
        {
            Assert.ThrowsException<MultipleIdentitiesFoundException>(() => ValueConverter.Map(DisplayNames));
        }
    }

    [TestClass]
    public class Given_multiple_combostrings : Given_multiple_display_names
    {
        public override void Given()
        {
            base.Given();
            DisplayNames = new[] { "Peter Lavallee <pelavall@microsoft.com>", "Jason Weber <jweber@microsoft.com>" };
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public new void Converted_value_contains_a_expected_number_of_results()
        {
            var kvp = (Dictionary<string, object>)ConvertedValue;
            kvp.Count.ShouldEqual(DisplayNames.Length);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public new void Converted_value_result_is_expected_value()
        {
            ConvertedValue.ShouldBeType<Dictionary<string, object>>();
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
            DisplayName = "Peter Lavallee";
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Converted_value_result_is_expected_value()
        {
            var kvp = (string)ConvertedValue;
            kvp.ShouldEqual("pelavall");
        }
    }

    [TestClass]
    public class Given_a_single_combostring : Given_a_single_displayname
    {
        /// <inheritdoc />
        public override void Given()
        {
            base.Given();
            DisplayName = "Peter Lavallee <pelavall@microsoft.com>";
        }
    }

    [TestClass]
    public class Given_a_single_display_name_with_multiple_identities : SingleDisplayNameContextSpecification
    {
        /// <inheritdoc />
        public override void Given()
        {
            base.Given();
            DisplayName = "Jason Weber";
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Converted_value_result_is_expected_value()
        {
            ConvertedValue.ShouldBeNull();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [Ignore]
        public new void Converted_value_contains_a_single_result()
        {
            Assert.Inconclusive();
        }

        public override void When()
        {
            Assert.ThrowsException<MultipleIdentitiesFoundException>(() => ValueConverter.Map(DisplayName));
        }
    }

    [TestClass]
    public class Given_a_single_combostring_with_multiple_identities : Given_a_single_display_name_with_multiple_identities
    {
        /// <inheritdoc />
        public override void Given()
        {
            base.Given();
            DisplayName = "Jason Weber <jweber@microsoft.com>";
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
            ((string)ConvertedValue).ShouldEqual("jweber", Comparer.OrdinalIgnoreCase);
        }
    }
}

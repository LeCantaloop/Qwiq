using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Mapper.Identity
{
    [TestClass]
    public class Given_multiple_display_names : MultipleDisplayNameContextSpecification
    {
        public override void Given()
        {
            base.Given();
            DisplayNames = new[]{"Peter Lavallee", "Jason Weber"};
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Converted_value_result_is_expected_value()
        {
            var kvp = ((Dictionary<string, string>)ConvertedValue);
            kvp.First().Value.ShouldEqual("pelavall");
            kvp.Skip(1).Single().Value.ShouldEqual("jweber");
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
            var kvp = ((Dictionary<string, string>)ConvertedValue);
            kvp.First().Value.ShouldEqual("pelavall");
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
            var kvp = ((Dictionary<string, string>)ConvertedValue);
            kvp.First().Value.ShouldEqual("jweber");
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
    }
}

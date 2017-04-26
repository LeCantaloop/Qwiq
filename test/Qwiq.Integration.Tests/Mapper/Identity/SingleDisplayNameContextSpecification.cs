using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Mapper.Identity
{
    public class SingleDisplayNameContextSpecification : DisplayNameToAliasConverterContextSpecification
    {
        protected internal string DisplayName { get; set; }

        /// <inheritdoc />
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void Converted_value_contains_a_single_result()
        {
            var kvp = (Dictionary<string, string>)ConvertedValue;
            kvp.Count.ShouldEqual(1);
        }

        /// <inheritdoc />
        public override void When()
        {
            ConvertedValue = TimedAction(() => ValueConverter.Map(DisplayName), "SOAP", "Map");
        }
    }
}
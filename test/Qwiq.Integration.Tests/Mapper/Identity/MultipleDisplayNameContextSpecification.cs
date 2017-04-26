using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Mapper.Identity
{
    public class MultipleDisplayNameContextSpecification : DisplayNameToAliasConverterContextSpecification
    {
        protected internal string[] DisplayNames { get; set; }

        [TestMethod]
        public void Converted_value_contains_a_expected_number_of_results()
        {
            var kvp = (Dictionary<string, string>)ConvertedValue;
            kvp.Count.ShouldEqual(DisplayNames.Length);
        }

        /// <inheritdoc />
        public override void When()
        {
            ConvertedValue = TimedAction(() => ValueConverter.Map(DisplayNames), "SOAP", "Map");
        }
    }
}
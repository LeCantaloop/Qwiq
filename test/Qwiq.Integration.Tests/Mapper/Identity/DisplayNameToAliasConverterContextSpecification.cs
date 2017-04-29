using System.Collections.Generic;

using Microsoft.Qwiq.Identity;
using Microsoft.Qwiq.Identity.Soap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Mapper.Identity
{
    /// <exclude />
    public abstract class DisplayNameToAliasConverterContextSpecification : SoapIdentityManagementServiceContextSpecification
    {
        protected object ConvertedValue { get; set; }

        protected DisplayNameToAliasValueConverter ValueConverter { get; private set; }

        [TestMethod]
        public void Converted_value_is_expected_type()
        {
            ConvertedValue.ShouldBeType(typeof(Dictionary<string, string>));
        }

        /// <inheritdoc />
        public override void Given()
        {
            base.Given();
            ValueConverter = new DisplayNameToAliasValueConverter(Instance);
        }
    }
}
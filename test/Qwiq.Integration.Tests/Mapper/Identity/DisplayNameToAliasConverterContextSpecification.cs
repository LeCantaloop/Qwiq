using System.Collections.Generic;

using Qwiq.Identity;
using Qwiq.Identity.Soap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.Mapper.Identity
{
    /// <exclude />
    public abstract class DisplayNameToAliasConverterContextSpecification : SoapIdentityManagementServiceContextSpecification
    {
        protected object ConvertedValue { get; set; } = null!;

        protected DisplayNameToAliasValueConverter ValueConverter { get; private set; } = null!;

        /// <inheritdoc />
        public override void Given()
        {
            base.Given();
            ValueConverter = new DisplayNameToAliasValueConverter(Instance);
        }
    }
}
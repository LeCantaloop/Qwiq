using System.Collections.Generic;
using Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.Identity.Soap
{
    public abstract class SoapIdentityMapperContextSpecification<T> : TimedContextSpecification
    {
        protected IdentityAliasValueConverter Instance { get; set; }

        protected T Input { get; set; }

        protected T ActualOutput { get; set; }

        protected T ExpectedOutput { get; set; }

        /// <inheritdoc />
        public override void Given()
        {
            base.Given();

            var wis = TimedAction(() => IntegrationSettings.CreateSoapStore(), "SOAP", "WIS Create");
            var soapIms = TimedAction(() => wis.GetIdentityManagementService(), "SOAP", "IMS Create");
            Instance = new IdentityAliasValueConverter(soapIms, IntegrationSettings.TenantId, IntegrationSettings.Domains);
        }

        public override void When()
        {
#pragma warning disable IDE0019 // Use pattern matching
            var stringValue = Input as string;
            var stringArray = Input as IEnumerable<string>;
#pragma warning restore IDE0019 // Use pattern matching

            if (stringValue != null)
            {
                ActualOutput = TimedAction(() => (T)Instance.Map(stringValue), "SOAP", "Map");
            }
            else if (stringArray != null)
            {
                ActualOutput = TimedAction(() => (T)Instance.Map(stringArray), "SOAP", "Map");
            }
            else
            {
                ActualOutput = Input;
            }
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void the_actual_output_is_the_expected_output()
        {
            ActualOutput.ShouldEqual(ExpectedOutput);
        }
    }
}
using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Identity.Soap
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
            ActualOutput = TimedAction(() => (T)Instance.Map(Input), "SOAP", "Map");
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
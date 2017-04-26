using Microsoft.Qwiq.Identity.Soap;
using Microsoft.Qwiq.Tests.Common;

namespace Microsoft.Qwiq.Identity
{
    public abstract class SoapIdentityManagementServiceContextSpecification : TimedContextSpecification
    {
        protected IIdentityManagementService Instance { get; private set; }

        /// <inheritdoc />
        public override void Given()
        {
            var wis = TimedAction(() => IntegrationSettings.CreateSoapStore(), "SOAP", "WIS Create");
            Instance = TimedAction(() => wis.GetIdentityManagementService(), "SOAP", "IMS Create");
        }
    }
}
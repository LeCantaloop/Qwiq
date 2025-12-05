using Qwiq.Tests.Common;

namespace Qwiq.Identity.Soap
{
    public abstract class SoapIdentityManagementServiceContextSpecification : TimedContextSpecification
    {
        protected IIdentityManagementService Instance { get; private set; } = null!;

        /// <inheritdoc />
        public override void Given()
        {
            var wis = TimedAction(() => IntegrationSettings.CreateSoapStore(), "SOAP", "WIS Create");
            Instance = TimedAction(() => wis.GetIdentityManagementService(), "SOAP", "IMS Create");
        }
    }
}
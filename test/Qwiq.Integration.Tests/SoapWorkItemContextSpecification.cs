namespace Microsoft.Qwiq.Integration.Tests
{
    public abstract class SoapWorkItemContextSpecification : WorkItemContextSpecification<IWorkItemStore>
    {


        protected override IWorkItemStore Create()
        {
            return TimedAction(() => IntegrationSettings.CreateSoapStore(), "SOAP", "WIS Create");
        }
    }
}
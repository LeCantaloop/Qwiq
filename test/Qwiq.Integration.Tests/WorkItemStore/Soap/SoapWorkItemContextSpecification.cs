namespace Microsoft.Qwiq.WorkItemStore.Soap
{
    public abstract class SoapWorkItemContextSpecification : WorkItemContextSpecification<IWorkItemStore>
    {
        protected override IWorkItemStore Create()
        {
            return TimedAction(() => IntegrationSettings.CreateSoapStore(), "SOAP", "WIS Create");
        }
    }
}
namespace Qwiq.WorkItemStore.Soap
{
    public abstract class SoapWorkItemContextSpecification : WorkItemContextSpecification<IWorkItemStore>
    {
        protected override IWorkItemStore Create()
        {
            return TimedAction(() =>
            {
                try
                {
                    return IntegrationSettings.CreateSoapStore();
                }
                catch (Exception)
                {
                    return null;
                }
            }, "SOAP", "WIS Create");
        }
    }
}
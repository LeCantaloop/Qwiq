namespace Microsoft.Qwiq.Integration.Tests
{
    public abstract class RestWorkItemContextSpecification : WorkItemContextSpecification<IWorkItemStore>
    {
        protected override IWorkItemStore Create()
        {
            return TimedAction(() => IntegrationSettings.CreateRestStore(), "REST", "WIS Create");
        }
    }
}
namespace Microsoft.Qwiq.WorkItemStore
{
    public abstract class RestWorkItemContextSpecification : WorkItemContextSpecification<IWorkItemStore>
    {
        protected override IWorkItemStore Create()
        {
            return TimedAction(() => IntegrationSettings.CreateRestStore(), "REST", "WIS Create");
        }
    }
}
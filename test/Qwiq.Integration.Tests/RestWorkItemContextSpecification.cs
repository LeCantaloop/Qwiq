namespace Microsoft.Qwiq.Integration.Tests
{
    public abstract class RestWorkItemContextSpecification : WorkItemContextSpecification<IWorkItemStore>
    {
        

        protected override IWorkItemStore Create()
        {
            var options = AuthenticationOptions;
            options.ClientType = ClientType.Rest;

            return TimedAction(() => Rest.WorkItemStoreFactory.Instance.Create(options), "REST", "WIS Create");
        }
    }
}
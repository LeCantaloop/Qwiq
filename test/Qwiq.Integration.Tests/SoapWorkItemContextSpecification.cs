namespace Microsoft.Qwiq.Integration.Tests
{
    public abstract class SoapWorkItemContextSpecification : WorkItemContextSpecification<IWorkItemStore>
    {


        protected override IWorkItemStore Create()
        {
            var options = AuthenticationOptions;
            options.ClientType = ClientType.Soap;

            return TimedAction(() => Soap.WorkItemStoreFactory.Instance.Create(options), "SOAP", "WIS Create");
        }
    }
}
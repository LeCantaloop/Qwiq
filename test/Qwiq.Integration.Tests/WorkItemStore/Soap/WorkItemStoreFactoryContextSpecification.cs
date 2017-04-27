using Microsoft.Qwiq.Tests.Common;

namespace Microsoft.Qwiq.WorkItemStore.Soap
{
    public abstract class WorkItemStoreFactoryContextSpecification : TimedContextSpecification
    {
        protected IWorkItemStoreFactory Instance { get; private set; }

        protected IWorkItemStore WorkItemStore { get; private set; }

        public override void Cleanup()
        {
            TimedAction(() => WorkItemStore?.Dispose(), "SOAP", "WIS Dispose");

            base.Cleanup();
        }

        public abstract IWorkItemStore Create();

        public override void Given()
        {
            Instance = Client.Soap.WorkItemStoreFactory.Default;
            WorkItemStore = TimedAction(Create, "SOAP", "WIS Create");
            base.Given();
        }
    }
}
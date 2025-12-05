using Qwiq.Tests.Common;

namespace Qwiq.WorkItemStore.Soap
{
    public abstract class WorkItemStoreFactoryContextSpecification : TimedContextSpecification
    {
        protected IWorkItemStoreFactory Instance { get; private set; } = null!;

        protected IWorkItemStore WorkItemStore { get; private set; } = null!;

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
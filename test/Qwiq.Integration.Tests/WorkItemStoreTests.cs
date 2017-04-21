using Microsoft.Qwiq.Tests.Common;

namespace Microsoft.Qwiq.Integration.Tests
{
    public abstract class WorkItemStoreTests<T> : TimedContextSpecification
        where T : IWorkItemStore
    {
        internal IQueryFactory QueryFactory;

        protected T WorkItemStore;

        public override void Cleanup()
        {
            WorkItemStore?.Dispose();
            base.Cleanup();
        }

        public override void Given()
        {
            WorkItemStore = Create();
        }

        protected abstract T Create();
    }
}
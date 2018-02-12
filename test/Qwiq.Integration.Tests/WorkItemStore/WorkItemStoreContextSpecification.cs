using Qwiq.Tests.Common;

namespace Qwiq.WorkItemStore
{
    public abstract class WorkItemStoreContextSpecification<T> : TimedContextSpecification
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
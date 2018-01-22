using Should;

namespace Microsoft.Qwiq.WorkItemStore.WorkItem
{
    public abstract class WorkItemWithLinksContextSpecification : WorkItemStoreComparisonContextSpecification
    {
        // Choose a work item having high External/Hyper/Related link/Attached file count.
        // The item selected below has 147 related, 14 external, 8 hyper, 8 attached
        private const int Id = 6413554;

        public override void When()
        {
            SoapResult.WorkItem = TimedAction(() => SoapResult.WorkItemStore.Query(Id), "SOAP", "Query By Id");
            RestResult.WorkItem = TimedAction(() => RestResult.WorkItemStore.Query(Id), "REST", "Query By Id");            
        }
    }
}
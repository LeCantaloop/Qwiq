using Should;

namespace Qwiq.WorkItemStore.WorkItem
{
    public abstract class WorkItemWithLinksContextSpecification : WorkItemStoreComparisonContextSpecification
    {
        // Choose a work item having high External/Hyper/Related link/Attached file count.
        // The sandbox item (ID 5) has 2 related links. More links can be added as needed.
        private const int Id = TestData.WorkItemWithLinksId;

        public override void When()
        {
            SoapResult.WorkItem = TimedAction(() => SoapResult.WorkItemStore!.Query(Id), "SOAP", "Query By Id");
            RestResult.WorkItem = TimedAction(() => RestResult.WorkItemStore!.Query(Id), "REST", "Query By Id");
        }
    }
}
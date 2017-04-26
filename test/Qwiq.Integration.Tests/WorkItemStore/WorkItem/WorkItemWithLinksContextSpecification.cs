using Should;

namespace Microsoft.Qwiq.WorkItemStore.WorkItem
{
    public abstract class WorkItemWithLinksContextSpecification : WorkItemStoreComparisonContextSpecification
    {
        private const int Id = 104268;

        public override void When()
        {
            SoapResult.WorkItem = TimedAction(() => SoapResult.WorkItemStore.Query(Id), "SOAP", "Query By Id");
            RestResult.WorkItem = TimedAction(() => RestResult.WorkItemStore.Query(Id), "REST", "Query By Id");

            SoapResult.WorkItem.ExternalLinkCount.ShouldBeGreaterThan(0);
            SoapResult.WorkItem.HyperlinkCount.ShouldBeGreaterThan(0);
            SoapResult.WorkItem.AttachedFileCount.ShouldBeGreaterThan(0);
            SoapResult.WorkItem.RelatedLinkCount.ShouldBeGreaterThan(0);
        }
    }
}
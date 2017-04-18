using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Integration.Tests
{
    [TestClass]
    public class Given_a_WorkItem_with_Links : WorkItemWithLinksContextSpecification
    {
        [TestMethod]
        public void links_from_both_implementations_are_equal()
        {
            RestResult.WorkItem.Links.ShouldContainOnly(SoapResult.WorkItem.Links);
        }
    }

    public abstract class WorkItemWithLinksContextSpecification : WorkItemStoreComparisonContextSpecification
    {
        private const int Id = 123456;

        public override void When()
        {
            SoapResult.WorkItem = TimedAction(() => SoapResult.WorkItemStore.Query(Id), "SOAP", "Query By Id");
            RestResult.WorkItem = TimedAction(() => RestResult.WorkItemStore.Query(Id), "REST", "Query By Id");
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.WorkItemStore.WorkItem
{
    public abstract class SingleWorkItemComparisonContextSpecification : WorkItemStoreComparisonContextSpecification
    {
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void WorkItem_is_equal()
        {
            RestResult.WorkItem.ShouldEqual(SoapResult.WorkItem);
            RestResult.WorkItem.GetHashCode().ShouldEqual(SoapResult.WorkItem.GetHashCode());
        }
    }
}
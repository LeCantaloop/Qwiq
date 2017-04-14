using Microsoft.Qwiq.Core.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Integration.Tests
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

    public abstract class WorkItemStoreComparisonContextSpecification : TimedContextSpecification
    {
        protected IWorkItemStore Rest => RestResult.WorkItemStore;

        protected Result RestResult { get; private set; }

        protected IWorkItemStore Soap => SoapResult.WorkItemStore;

        protected Result SoapResult { get; private set; }

        public override void Cleanup()
        {
            Rest?.Dispose();
            Soap?.Dispose();

            base.Cleanup();
        }

        public override void Given()
        {
            SoapResult = new Result { WorkItemStore = TimedAction(() => IntegrationSettings.CreateSoapStore(), "SOAP", "Create WIS") };

            RestResult = new Result { WorkItemStore = TimedAction(() => IntegrationSettings.CreateRestStore(), "REST", "Create WIS") };
        }
    }
}
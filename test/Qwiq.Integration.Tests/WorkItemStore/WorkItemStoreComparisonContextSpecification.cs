using System;

using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.WorkItemStore
{
    [DeploymentItem("Microsoft.WITDataStore32.dll")]
    [DeploymentItem("Microsoft.WITDataStore64.dll")]
    [DeploymentItem("Microsoft.TeamFoundation.WorkItemTracking.Client.dll")]
    public abstract class WorkItemStoreComparisonContextSpecification : TimedContextSpecification
    {
        protected IWorkItemStore Rest => RestResult.WorkItemStore;

        protected Result RestResult { get; private set; }

        protected IWorkItemStore Soap => SoapResult.WorkItemStore;

        protected Result SoapResult { get; private set; }

        public override void Cleanup()
        {
            RestResult?.Dispose();
            SoapResult?.Dispose();

            base.Cleanup();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public override void Given()
        {
            SoapResult = new Result { WorkItemStore = TimedAction(() => IntegrationSettings.CreateSoapStore(), "SOAP", "Create WIS") };

            RestResult = new Result { WorkItemStore = TimedAction(() => IntegrationSettings.CreateRestStore(), "REST", "Create WIS") };
        }

        [TestMethod]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        [TestCategory("localOnly")]
        public void REST_implementation_is_faster_than_SOAP()
        {
            Durations.TryGetValue("SOAP", out TimeSpan soapTime);
            Durations.TryGetValue("REST", out TimeSpan restTime);

            restTime.ShouldBeLessThan(soapTime);
        }
    }
}
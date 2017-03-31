using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Core.Tests
{
    [TestClass]
    public class Given_a_WorkItem_from_each_client : IntegrationContextSpecificationSpecification
    {
        private const int Id = 10726528;

        public override void When()
        {
            SoapResult.WorkItem = TimedAction(() => SoapResult.WorkItemStore.Query(Id), "SOAP", "Query By Id");
            RestResult.WorkItem = TimedAction(() => RestResult.WorkItemStore.Query(Id), "REST", "Query By Id");
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void WorkItemType_is_equal()
        {
            RestResult.WorkItem.Type.ShouldEqual(SoapResult.WorkItem.Type);
        }
    }

    [TestClass]
    public class Given_a_WorkItem_from_each_client_by_AsOf : IntegrationContextSpecificationSpecification
    {
        private const int Id = 10726528;

        public override void When()
        {
            DateTime t = DateTime.UtcNow;

            SoapResult.WorkItem = TimedAction(() => SoapResult.WorkItemStore.Query(Id, t), "SOAP", "Query By Id");
            RestResult.WorkItem = TimedAction(() => RestResult.WorkItemStore.Query(Id, t), "REST", "Query By Id");
        }


    }
}
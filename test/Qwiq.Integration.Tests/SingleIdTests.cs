using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Integration.Tests
{
    [TestClass]
    public class Given_a_WorkItem_from_each_client_by_Id : SingleWorkItemComparisonContextSpecification
    {
        private const int Id = 10726528;

        public override void When()
        {
            SoapResult.WorkItem = TimedAction(() => SoapResult.WorkItemStore.Query(Id), "SOAP", "Query By Id");
            RestResult.WorkItem = TimedAction(() => RestResult.WorkItemStore.Query(Id), "REST", "Query By Id");
        }
    }

    [TestClass]
    public class Given_a_WorkItem_from_each_client_by_Id_at_AsOf : SingleWorkItemComparisonContextSpecification
    {
        private const int Id = 10726528;

        public override void When()
        {
            var t = DateTime.UtcNow;

            SoapResult.WorkItem = TimedAction(() => SoapResult.WorkItemStore.Query(Id, t), "SOAP", "Query By Id");
            RestResult.WorkItem = TimedAction(() => RestResult.WorkItemStore.Query(Id, t), "REST", "Query By Id");
        }
    }
}
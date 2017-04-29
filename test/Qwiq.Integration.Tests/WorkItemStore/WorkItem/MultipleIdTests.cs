using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Qwiq.WorkItemStore.WorkItem
{
    [TestClass]
    public class Given_WorkItems_from_each_client_by_IDs : SingleWorkItemComparisonContextSpecification
    {
        private const int Id = 10726528;

        public override void When()
        {
            SoapResult.WorkItem = SoapResult.WorkItemStore.Query(new[] { Id }).Single();
            RestResult.WorkItem = RestResult.WorkItemStore.Query(new[] { Id }).Single();
        }
    }

    [TestClass]
    public class Given_WorkItems_from_each_client_by_IDs_at_AsOf : SingleWorkItemComparisonContextSpecification
    {
        private const int Id = 10726528;

        public override void When()
        {
            var t = DateTime.UtcNow;

            SoapResult.WorkItem = SoapResult.WorkItemStore.Query(new[] { Id }, t).Single();
            RestResult.WorkItem = RestResult.WorkItemStore.Query(new[] { Id }, t).Single();
        }
    }
}
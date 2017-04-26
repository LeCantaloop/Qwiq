using System;
using System.Globalization;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Qwiq.WorkItemStore.WorkItem


{
    [TestClass]
    public class Given_a_WorkItem_from_each_client_by_WIQL : SingleWorkItemComparisonContextSpecification
    {
        private const int Id = 10726528;

        private static readonly string Wiql = ((FormattableString)$"SELECT {string.Join(", ", CoreFieldRefNames.All)} FROM WorkItems WHERE [System.Id] = {Id}").ToString(CultureInfo.InvariantCulture);

        public override void When()
        {
            RestResult.WorkItem = TimedAction(()=> RestResult.WorkItemStore.Query(Wiql).Single(), "REST", "Query");
            SoapResult.WorkItem = TimedAction(() => SoapResult.WorkItemStore.Query(Wiql).Single(), "SOAP", "Query");
        }
    }
}
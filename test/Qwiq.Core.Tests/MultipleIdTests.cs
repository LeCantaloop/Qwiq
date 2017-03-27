using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Qwiq.Core.Tests
{
    [TestClass]
    public class MultipleIdTests : IntegrationContextSpecification
    {
        private const int Id = 10726528;

        public override void When()
        {
            SoapResult.WorkItem = SoapResult.WorkItemStore.Query(new[] { Id }).Single();
            RestResult.WorkItem = RestResult.WorkItemStore.Query(new[] { Id }).Single();
        }
    }
}
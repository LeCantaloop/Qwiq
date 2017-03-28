using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Qwiq.Core.Tests
{
    [TestClass]
    public class SingleIdTests : IntegrationContextSpecificationSpecification
    {
        private const int Id = 10726528;

        public override void When()
        {
            SoapResult.WorkItem = SoapResult.WorkItemStore.Query(Id);
            RestResult.WorkItem = RestResult.WorkItemStore.Query(Id);
        }
    }
}
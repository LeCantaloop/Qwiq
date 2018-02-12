using System.Linq;

using Qwiq.Identity;
using Qwiq.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.WorkItemStore.Linq
{
    [TestClass]
    public class Given_WorkItems_queried_by_LINQ_on_AssignedTo_by_UPN : LinqContextSpecification
    {
        /// <inheritdoc />
        public override void When()
        {
            RestResult.WorkItems = RestQueryable.Where(i => i.AssignedTo == "rimuri@microsoft.com").ToArray().ToWorkItemCollection();
            SoapResult.WorkItems = SoapQueryable.Where(i => i.AssignedTo == "rimuri@microsoft.com").ToArray().ToWorkItemCollection();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void the_results_are_equal()
        {
            RestResult.WorkItems.ShouldContainOnly(SoapResult.WorkItems);
        }
    }

    [TestClass]
    public class Given_WorkItems_queried_by_LINQ_on_AssignedTo_with_IdentityVisitor_by_alias : IdentityMapperContextSpecification
    {
        /// <inheritdoc />
        public override void When()
        {
            RestResult.WorkItems = RestQueryable.Where(i => i.AssignedTo == "rimuri").ToArray().ToWorkItemCollection();
            SoapResult.WorkItems = SoapQueryable.Where(i => i.AssignedTo == "rimuri").ToArray().ToWorkItemCollection();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void SOAP_returned_results()
        {
            SoapResult.WorkItems.Count.ShouldBeGreaterThan(0);
        }
    }
}

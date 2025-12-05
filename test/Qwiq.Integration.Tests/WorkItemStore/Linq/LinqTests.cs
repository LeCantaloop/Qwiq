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
            RestResult.WorkItems = RestQueryable.Where(i => i.AssignedTo == TestData.TestUserUpn).ToArray().ToWorkItemCollection();
            SoapResult.WorkItems = SoapQueryable.Where(i => i.AssignedTo == TestData.TestUserUpn).ToArray().ToWorkItemCollection();
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
    public class Given_WorkItems_queried_by_LINQ_on_CreatedBy_by_UPN : LinqContextSpecification
    {
        /// <inheritdoc />
        public override void When()
        {
            // Query by CreatedBy using UPN because the sandbox has work items created by the test user
            // but may not have work items assigned to them
            RestResult.WorkItems = RestQueryable.Where(i => i.CreatedBy == TestData.TestUserUpn).ToArray().ToWorkItemCollection();
            SoapResult.WorkItems = SoapQueryable.Where(i => i.CreatedBy == TestData.TestUserUpn).ToArray().ToWorkItemCollection();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void the_results_are_equal()
        {
            RestResult.WorkItems.ShouldContainOnly(SoapResult.WorkItems);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void SOAP_returned_results()
        {
            SoapResult.WorkItems.Count.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        public void REST_returned_results()
        {
            RestResult.WorkItems.Count.ShouldBeGreaterThan(0);
        }
    }
}

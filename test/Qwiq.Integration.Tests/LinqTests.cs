using System.Linq;

using Microsoft.Qwiq.Linq;
using Microsoft.Qwiq.Linq.Visitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Integration.Tests
{
    public abstract class LinqContextSpecification : WorkItemStoreComparisonContextSpecification
    {
        protected IOrderedQueryable<IWorkItem> SoapQueryable { get; private set; }
        protected IOrderedQueryable<IWorkItem> RestQueryable { get; private set; }



        /// <inheritdoc />
        public override void Given()
        {
            base.Given();


            var soapBuilder = new WiqlQueryBuilder();
            var soapQp = new TeamFoundationServerWorkItemQueryProvider(Soap, soapBuilder);
            SoapQueryable = new Query<IWorkItem>(soapQp, soapBuilder);

            var restBuilder = new WiqlQueryBuilder();
            var restQp = new TeamFoundationServerWorkItemQueryProvider(Rest, restBuilder);
            RestQueryable = new Query<IWorkItem>(restQp, restBuilder);
        }
    }

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
}

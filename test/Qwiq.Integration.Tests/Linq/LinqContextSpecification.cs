using System.Linq;

using Microsoft.Qwiq.Linq.Visitors;
using Microsoft.Qwiq.WorkItemStore;

namespace Microsoft.Qwiq.Linq
{
    public abstract class LinqContextSpecification : WorkItemStoreComparisonContextSpecification
    {
        protected IOrderedQueryable<IWorkItem> RestQueryable { get; private set; }

        protected IOrderedQueryable<IWorkItem> SoapQueryable { get; private set; }

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
}
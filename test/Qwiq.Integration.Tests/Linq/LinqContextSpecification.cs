using System.Linq;

using Qwiq.Linq.Visitors;
using Qwiq.WorkItemStore;

namespace Qwiq.Linq
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
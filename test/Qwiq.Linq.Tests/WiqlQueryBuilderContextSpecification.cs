using System.Linq;

using Microsoft.Qwiq.Linq.Visitors;
using Microsoft.Qwiq.Mocks;
using Microsoft.Qwiq.Tests.Common;

namespace Microsoft.Qwiq.Linq
{
    public abstract class WiqlQueryBuilderContextSpecification : ContextSpecification
    {
        protected string Actual;

        protected string Expected;

        protected IOrderedQueryable<IWorkItem> Query;

        protected TeamFoundationServerWorkItemQueryProvider QueryProvider { get; set; }

        protected WiqlQueryBuilder WiqlQueryBuilder { get; set; }

        public override void Given()
        {
            base.Given();
            WiqlQueryBuilder = new WiqlQueryBuilder();
            QueryProvider = new TeamFoundationServerWorkItemQueryProvider(new MockWorkItemStore(), WiqlQueryBuilder);
            Query = new Query<IWorkItem>(QueryProvider, WiqlQueryBuilder);
        }
    }
}
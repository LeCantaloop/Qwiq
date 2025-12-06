using System.Linq;

using Qwiq.Linq.Visitors;
using Qwiq.Mocks;
using Qwiq.Tests.Common;

namespace Qwiq.Linq
{
    public abstract class WiqlQueryBuilderContextSpecification : ContextSpecification
    {
        protected string Actual { get; set; } = null!;

        protected string Expected { get; set; } = null!;

        protected IOrderedQueryable<IWorkItem> Query { get; set; } = null!;

        protected TeamFoundationServerWorkItemQueryProvider QueryProvider { get; set; } = null!;

        protected WiqlQueryBuilder WiqlQueryBuilder { get; set; } = null!;

        public override void Given()
        {
            base.Given();
            WiqlQueryBuilder = new WiqlQueryBuilder();
            QueryProvider = new TeamFoundationServerWorkItemQueryProvider(new MockWorkItemStore(), WiqlQueryBuilder);
            Query = new Query<IWorkItem>(QueryProvider, WiqlQueryBuilder);
        }
    }
}
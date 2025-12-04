using System.Linq;

using Qwiq.Linq.Visitors;
using Qwiq.Mocks;
using Qwiq.Tests.Common;

namespace Qwiq.Linq
{
    public abstract class WiqlQueryBuilderContextSpecification : ContextSpecification
    {
        protected string Actual = null!;

        protected string Expected = null!;

        protected IOrderedQueryable<IWorkItem> Query = null!;

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
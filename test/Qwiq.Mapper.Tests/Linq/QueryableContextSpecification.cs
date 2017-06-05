using System.Linq;

using Microsoft.Qwiq.Linq;
using Microsoft.Qwiq.Linq.Visitors;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mocks;
using Microsoft.Qwiq.Tests.Common;

namespace Microsoft.Qwiq.Mapper.Linq
{
    public abstract class QueryableContextSpecification<T> : ContextSpecification
    {
        protected IOrderedQueryable<T> Query;

        protected virtual IWorkItemStore CreateWorkItemStore()
        {
            return new MockWorkItemStore();
        }

        protected virtual IFieldMapper CreateFieldMapper()
        {
            return new CachingFieldMapper(new FieldMapper());
        }

        protected virtual IPropertyInspector CreatePropertyInspector()
        {
            return new PropertyInspector(new PropertyReflector());
        }

        public override void Given()
        {
            var workItemStore = CreateWorkItemStore();
            var fieldMapper = CreateFieldMapper();

            var propertyInspector = CreatePropertyInspector();

            var mapperStrategies = new IWorkItemMapperStrategy[]
            {
                new AttributeMapperStrategy(propertyInspector),
                new WorkItemLinksMapperStrategy(propertyInspector, workItemStore)
            };

            var builder = new WiqlQueryBuilder(new WiqlTranslator(fieldMapper), new PartialEvaluator(), new QueryRewriter());
            var mapper = new WorkItemMapper(mapperStrategies);

            var queryProvider = new MapperTeamFoundationServerWorkItemQueryProvider(workItemStore, builder, mapper);
            Query = new Query<T>(queryProvider, builder);
        }
    }
}

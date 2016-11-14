using System.Collections.Generic;
using System.Linq;
using Microsoft.Qwiq.Core.Tests;
using Microsoft.Qwiq.Linq;
using Microsoft.Qwiq.Linq.Visitors;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mocks;

namespace Microsoft.Qwiq.Mapper.Tests
{
    public abstract class QueryTestsBase<T> : ContextSpecification
    {
        protected IOrderedQueryable<T> Query;

        protected virtual IWorkItemStore CreateWorkItemStore()
        {
            var workItems = new List<IWorkItem>
            {
                new MockWorkItem("SimpleMockWorkItem", new Dictionary<string, object>
                    {
                        {"ID", 1},
                        {"IntField", 2}
                    }),
                new MockWorkItem("SimpleMockWorkItem", new Dictionary<string, object>
                    {
                        {"ID", 2},
                        {"IntField", 4}
                    })
,
                new MockWorkItem("SimpleMockWorkItem", new Dictionary<string, object>
                    {
                        {"ID", 3},
                        {"IntField", 3}
                    })
,
                new MockWorkItem("SimpleMockWorkItem", new Dictionary<string, object>
                    {
                        {"ID", 4},
                        {"IntField", 4}
                    })
,
                new MockWorkItem("SimpleMockWorkItem", new Dictionary<string, object>
                    {
                        {"ID", 5},
                        {"IntField", 5}
                    })

            };

            var links = new[] {
                new MockWorkItemLinkInfo(0, 3),
                new MockWorkItemLinkInfo(3, 1),
                new MockWorkItemLinkInfo(3, 2),
                new MockWorkItemLinkInfo(0, 4),
                new MockWorkItemLinkInfo(0, 5)
            };

            return new MockWorkItemStore(workItems, links);
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
                new AttributeMapperStrategy(propertyInspector, new TypeParser()),
                new WorkItemLinksMapperStrategy(propertyInspector, workItemStore)
            };

            var builder = new WiqlQueryBuilder(new WiqlTranslator(fieldMapper), new PartialEvaluator(), new QueryRewriter());
            var mapper = new WorkItemMapper(mapperStrategies);

            var queryProvider = new MapperTeamFoundationServerWorkItemQueryProvider(workItemStore, builder, mapper);
            Query = new Query<T>(queryProvider, builder);
        }
    }
}

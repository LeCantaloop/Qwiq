using System.Collections.Generic;
using System.Linq;
using Microsoft.Qwiq.Core.Tests;
using Microsoft.Qwiq.Linq;
using Microsoft.Qwiq.Linq.Visitors;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mocks;

namespace Microsoft.Qwiq.Mapper.Tests
{
    public abstract class QueryTestsBase : ContextSpecification
    {
        protected IWorkItemMapper Mapper;
        protected IWiqlQueryBuilder Builder;
        protected IPropertyReflector PropertyReflector;
        protected IWorkItemStore WorkItemStore;
        protected IQueryProvider QueryProvider;
        protected IFieldMapper FieldMapper;

        public override void Given()
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

            WorkItemStore = new MockWorkItemStore(workItems, links);
            FieldMapper = new CachingFieldMapper(new FieldMapper());

            var propertyInspector = new PropertyInspector(PropertyReflector);
            var mapperStrategies = new IWorkItemMapperStrategy[]
            {
                new AttributeMapperStrategy(propertyInspector,
                    new TypeParser()),
                    new WorkItemLinksMapperStrategy(propertyInspector, WorkItemStore)
            };

            Builder = new WiqlQueryBuilder(new WiqlTranslator(FieldMapper), new PartialEvaluator(), new QueryRewriter());
            Mapper = new WorkItemMapper(mapperStrategies);
        }

        public override void When()
        {
            QueryProvider = new MapperTeamFoundationServerWorkItemQueryProvider(WorkItemStore, Builder, Mapper);
        }
    }
}

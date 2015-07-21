using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq.Linq.Tests.Mocks;
using Microsoft.IE.Qwiq.Linq.Visitors;
using Microsoft.IE.Qwiq.Mapper;
using Microsoft.IE.Qwiq.Mapper.Attributes;

namespace Microsoft.IE.Qwiq.Linq.Tests
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
                new MockWorkItem
                {
                    Id = 1,
                    Type = new MockWorkItemType
                    {
                        Name = "SimpleMockWorkItem"
                    },
                    Properties = new Dictionary<string, object>
                    {
                        {"ID", 1},
                        {"IntField", 2}
                    }
                },
                new MockWorkItem
                {
                    Id = 2,
                    Type = new MockWorkItemType
                    {
                        Name = "SimpleMockWorkItem"
                    },
                    Properties = new Dictionary<string, object>
                    {
                        {"ID", 2},
                        {"IntField", 4}
                    }
                },
                new MockWorkItem
                {
                    Id = 3,
                    Type = new MockWorkItemType
                    {
                        Name = "SimpleMockWorkItem"
                    },
                    Properties = new Dictionary<string, object>
                    {
                        {"ID", 3},
                        {"IntField", 3}
                    }
                },
                new MockWorkItem
                {
                    Id = 4,
                    Type = new MockWorkItemType
                    {
                        Name = "SimpleMockWorkItem"
                    },
                    Properties = new Dictionary<string, object>
                    {
                        {"ID", 4},
                        {"IntField", 4}
                    }
                },
                new MockWorkItem
                {
                    Id = 5,
                    Type = new MockWorkItemType
                    {
                        Name = "SimpleMockWorkItem"
                    },
                    Properties = new Dictionary<string, object>
                    {
                        {"ID", 5},
                        {"IntField", 5}
                    }
                }
            };

            var links = new[] {
                new MockWorkItemLinkInfo
                {
                    SourceId = 0,
                    TargetId = 3
                },
                new MockWorkItemLinkInfo
                {
                    SourceId = 3,
                    TargetId = 1
                },
                new MockWorkItemLinkInfo
                {
                    SourceId = 3,
                    TargetId = 2
                },
                new MockWorkItemLinkInfo
                {
                    SourceId = 0,
                    TargetId = 4
                },
                new MockWorkItemLinkInfo
                {
                    SourceId = 0,
                    TargetId = 5
                }
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
            Mapper = new WorkItemMapper(FieldMapper, mapperStrategies);
        }

        public override void When()
        {
            QueryProvider = new TeamFoundationServerWorkItemQueryProvider(WorkItemStore, Builder, Mapper);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq.Linq;
using Microsoft.IE.Qwiq.Linq.Visitors;
using Microsoft.IE.Qwiq.Mapper;
using Microsoft.IE.Qwiq.Mapper.Attributes;
using Microsoft.IE.Qwiq.Relatives.Linq;
using Microsoft.IE.Qwiq.Relatives.Mapper;
using Microsoft.IE.Qwiq.Relatives.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.IE.Qwiq.Relatives.Tests
{
    public abstract class RelativesAwareTeamFoundationServerWorkItemQueryProviderContextSpecification : ContextSpecification
    {
        protected Query<SimpleMockModel> Query;
        protected IEnumerable<KeyValuePair<SimpleMockModel, IEnumerable<SimpleMockModel>>> Actual;

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

            var workItemStore = new MockWorkItemStore(workItems, links);
            var fieldMapper = new CachingFieldMapper(new FieldMapper());
            var propertyReflector = new PropertyReflector();
            var propertyInspector = new PropertyInspector(propertyReflector);
            var builder = new WiqlQueryBuilder(new RelativesAwareWiqlTranslator(fieldMapper), new PartialEvaluator(), new RelativesAwareQueryRewriter());
            var mapperStrategies = new IWorkItemMapperStrategy[]
            {
                new AttributeMapperStrategy(propertyInspector,
                    new TypeParser()),
                    new WorkItemLinksMapperStrategy(propertyInspector, workItemStore),
                    new ParentIdMapperStrategy(workItemStore)
            };
            var mapper = new WorkItemMapper(fieldMapper, mapperStrategies);
            var queryProvider = new RelativesAwareTeamFoundationServerWorkItemQueryProvider(workItemStore, builder, mapper, fieldMapper);

            Query = new Query<SimpleMockModel>(queryProvider, builder);
        }
    }

    [TestClass]
    public class given_a_query_provider_when_a_parents_clause_is_used : RelativesAwareTeamFoundationServerWorkItemQueryProviderContextSpecification
    {
        public override void When()
        {
            Actual = Query.Parents<SimpleMockModel, SimpleMockModel>();
        }

        [TestMethod]
        public void item_with_id_of_1_has_a_parent_of_id_3()
        {
            Actual.Single(kvp => kvp.Key.ID == 3).Value.Count(wi => wi.ID == 1).ShouldEqual(1);
        }

        [TestMethod]
        public void item_with_id_of_5_has_no_parent()
        {
            Actual.Single(kvp => kvp.Key.ID == 5).Value.ShouldBeEmpty();
        }
    }

    [TestClass]
    public class given_a_query_provider_when_a_children_clause_is_used : RelativesAwareTeamFoundationServerWorkItemQueryProviderContextSpecification
    {
        public override void When()
        {
            Actual = Query.Children<SimpleMockModel, SimpleMockModel>();
        }

        [TestMethod]
        public void item_with_id_of_3_has_a_child_of_id_2()
        {
            Actual.Single(kvp => kvp.Key.ID == 3).Value.Count(wi => wi.ID == 2).ShouldEqual(1);
        }

        [TestMethod]
        public void item_with_id_of_5_has_no_children()
        {
            Actual.Single(kvp => kvp.Key.ID == 5).Value.ShouldBeEmpty();
        }
    }


    [TestClass]
    public class when_a_parents_clause_is_used_on_a_query_with_no_results : RelativesAwareTeamFoundationServerWorkItemQueryProviderContextSpecification
    {
        public override void When()
        {
            Actual = Query
                        .Where(item => item.ID == -1)
                        .Parents<SimpleMockModel, SimpleMockModel>();
        }

        [TestMethod]
        public void an_empty_set_is_returned()
        {
            Actual.ShouldBeEmpty();
        }
    }

    [TestClass]
    public class when_a_children_clause_is_used_on_a_query_with_no_results : RelativesAwareTeamFoundationServerWorkItemQueryProviderContextSpecification
    {
        public override void When()
        {
            Actual = Query
                        .Where(item => item.ID == -1)
                        .Children<SimpleMockModel, SimpleMockModel>();
        }

        [TestMethod]
        public void an_empty_set_is_returned()
        {
            Actual.ShouldBeEmpty();
        }
    }
}

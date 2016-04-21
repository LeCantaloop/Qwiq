using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq.Credentials;
using Microsoft.IE.Qwiq.Linq;
using Microsoft.IE.Qwiq.Linq.Visitors;
using Microsoft.IE.Qwiq.Mapper;
using Microsoft.IE.Qwiq.Mapper.Attributes;
using Microsoft.IE.Qwiq.Relatives.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.IE.Qwiq.Relatives.Tests
{
    public class LocalOnlyRelativesQueryTest : ContextSpecification
    {
        protected IEnumerable<KeyValuePair<Deliverable, IEnumerable<Task>>> Relatives;

        protected IQueryable<Task> Tasks { get; set; }
        protected IQueryable<Deliverable> Deliverables { get; set; }

        public override void Given()
        {
            var wis =
                WorkItemStoreFactory.GetInstance()
                    .Create(new Uri("https://microsoft.visualstudio.com/DefaultCollection"), CredentialsFactory.CreateAadCredentials());
            var propertyInspector = new PropertyInspector(new CachingPropertyReflector(new PropertyReflector()));
            var fieldMapper = new CachingFieldMapper(new FieldMapper());
            var typeParser = new TypeParser();
            var mappingStrategies = new IWorkItemMapperStrategy[]
            {
                new AttributeMapperStrategy(propertyInspector, typeParser)
            };
            var mapper = new WorkItemMapper(mappingStrategies);
            var wiqlQueryBuilder = new WiqlQueryBuilder(new RelativesAwareWiqlTranslator(fieldMapper), new PartialEvaluator(), new RelativesAwareQueryRewriter());
            var queryProvider = new RelativesAwareTeamFoundationServerWorkItemQueryProvider(wis, wiqlQueryBuilder, mapper, fieldMapper);
            Tasks = new Query<Task>(queryProvider, wiqlQueryBuilder);
            Deliverables = new Query<Deliverable>(queryProvider, wiqlQueryBuilder);
        }
    }

    [WorkItemType("Task")]
    public class Task : IIdentifiable
    {
        [FieldDefinition("Id")]
        public int Id { get; set; }
    }

    [WorkItemType("Deliverable")]
    public class Deliverable : IIdentifiable
    {
        [FieldDefinition("Id")]
        public int Id { get; set; }
    }

    [TestClass]
    public class given_a_live_vso_context_when_a_tasks_parents_are_retrieved : LocalOnlyRelativesQueryTest
    {
        public override void When()
        {
            Relatives = Tasks.Where(t => t.Id == 1542232).Parents<Deliverable, Task>().ToList();
        }

        [TestCategory("localOnly")]
        [TestMethod]
        public void the_returned_set_should_have_one_entry()
        {
            Relatives.Count().ShouldEqual(1);
        }

        [TestCategory("localOnly")]
        [TestMethod]
        public void the_returned_set_should_have_the_parent_as_the_key()
        {
            Relatives.Single().Key.Id.ShouldEqual(1523767);
        }

        [TestCategory("localOnly")]
        [TestMethod]
        public void the_returned_parent_should_have_a_single_child()
        {
            Relatives.Single().Value.Count().ShouldEqual(1);
        }

        [TestCategory("localOnly")]
        [TestMethod]
        public void the_returned_child_should_be_the_original_task()
        {
            Relatives.Single().Value.Single().Id.ShouldEqual(1542232);
        }
    }

    [TestClass]
    public class given_a_live_vso_context_when_the_deliverable_1523767s_children_are_retrieved : LocalOnlyRelativesQueryTest
    {
        public override void When()
        {
            Relatives = Deliverables.Where(d => d.Id == 1523767).Children<Deliverable, Task>().ToList();
        }

        [TestCategory("localOnly")]
        [TestMethod]
        public void the_returned_set_should_have_one_entry()
        {
            Relatives.Count().ShouldEqual(1);
        }

        [TestCategory("localOnly")]
        [TestMethod]
        public void the_returned_set_should_have_the_parent_as_the_key()
        {
            Relatives.Single().Key.Id.ShouldEqual(1523767);
        }

        [TestCategory("localOnly")]
        [TestMethod]
        public void the_returned_parent_should_have_4_Children()
        {
            Relatives.Single().Value.Count().ShouldEqual(4);
        }

        [TestCategory("localOnly")]
        [TestMethod]
        public void the_children_should_include_1542232()
        {
            Relatives.Single().Value.Count(t => t.Id == 1542232).ShouldEqual(1);
        }
    }

    [TestClass]
    public class given_a_live_vso_context_when_a_task_is_selected_and_projected : LocalOnlyRelativesQueryTest
    {
        [TestCategory("localOnly")]
        [TestMethod]
        public void the_projection_should_have_the_correct_contents()
        {
            Tasks.Where(t => t.Id == 1542232).Select(t => new { ABC = t.Id }).ToList().Single().ABC.ShouldEqual(1542232);
        }
    }
}
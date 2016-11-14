using System.Collections.Generic;
using System.Linq;
using Microsoft.Qwiq.Linq;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mapper.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Microsoft.Qwiq.Mapper.Tests
{
    public abstract class QueryProviderTests : QueryTestsBase
    {
        protected Query<SimpleMockModel> Query;

        public override void Given()
        {
            PropertyReflector = new PropertyReflector();
            base.Given();
        }

        public override void When()
        {
            base.When();
            Query = new Query<SimpleMockModel>(QueryProvider, Builder);
        }
    }

    public abstract class SelectTests : QueryProviderTests
    {
        protected object Expected;
        protected object Actual;
    }

    [TestClass]
    public class given_a_query_provider_when_a_select_clause_is_used : SelectTests
    {
        public override void When()
        {
            base.When();
            Expected = new[] { new { One = 2, Two = 2 }, new { One = 4, Two = 4 }, new { One = 3, Two = 3 }, new { One = 4, Two = 4 }, new { One = 5, Two = 5 } };
            Actual = Query.Select(item => new { One = item.IntField, Two = item.IntField }).ToArray();
        }

        [TestMethod]
        public void the_selected_objects_are_returned()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    public class given_a_query_provider_when_two_select_clauses_are_chained : SelectTests
    {
        public override void When()
        {
            base.When();
            Expected = new[] { new { ABC = 2 }, new { ABC = 4 }, new { ABC = 3 }, new { ABC = 4 }, new { ABC = 5 } };
            Actual = Query.Select(item => new { One = item.IntField, Two = item.IntField }).Select(item2 => new { ABC = item2.Two }).ToArray();
        }

        [TestMethod]
        public void the_selected_objects_are_returned()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    public class when_a_where_clause_includes_an_empty_contains_clause : QueryProviderTests
    {
        private IEnumerable<SimpleMockModel> _actual;
        private InstrumentedMockWorkItemStore _workItemStore;

        public override void Given()
        {
            base.Given();
            _workItemStore = new InstrumentedMockWorkItemStore(WorkItemStore);
            var queryProvider = new InstrumentedMockQueryProvider(new MapperTeamFoundationServerWorkItemQueryProvider(_workItemStore, Builder, Mapper));
            Query = new Query<SimpleMockModel>(queryProvider, Builder);
        }

        public override void When()
        {
            base.When();
            var list = Enumerable.Empty<int>().ToArray();
            _actual = Query.Where(item => list.Contains(item.IntField)).ToList();
        }

        [TestMethod]
        public void the_query_should_not_be_run()
        {
            _workItemStore.QueryCallCount.ShouldEqual(0);
        }

        [TestMethod]
        public void the_query_should_return_the_empty_result_set()
        {
            _actual.ShouldBeEmpty();
        }
    }
}


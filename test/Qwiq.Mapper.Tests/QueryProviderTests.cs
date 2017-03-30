using System.Collections.Generic;
using System.Linq;
using Microsoft.Qwiq.Mapper.Tests.Mocks;
using Microsoft.Qwiq.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Microsoft.Qwiq.Mapper.Tests
{
    public abstract class SelectTests : QueryTestsBase<SimpleMockModel>
    {
        protected object Expected;
        protected object Actual;

        protected override IWorkItemStore CreateWorkItemStore()
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

            var links = new[]
            {
                new MockWorkItemLinkInfo(0, 3),
                new MockWorkItemLinkInfo(3, 1),
                new MockWorkItemLinkInfo(3, 2),
                new MockWorkItemLinkInfo(0, 4),
                new MockWorkItemLinkInfo(0, 5)
            };

            var tpcMock = new MockTfsTeamProjectCollection();
            var projMock = new MockProject(workItems.Select(s=>s.Type).Distinct());

            return new MockWorkItemStore(tpcMock, projMock, null, workItems, links);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
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
    // ReSharper disable once InconsistentNaming
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
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_includes_an_empty_contains_clause : QueryTestsBase<SimpleMockModel>
    {
        private IEnumerable<SimpleMockModel> _actual;
        private InstrumentedMockWorkItemStore _workItemStore;

        protected override IWorkItemStore CreateWorkItemStore()
        {
            _workItemStore = new InstrumentedMockWorkItemStore(base.CreateWorkItemStore());
            return _workItemStore;
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


using System.Collections.Generic;
using System.Linq;

using Qwiq.Mapper.Linq;
using Qwiq.Mapper.Mocks;
using Qwiq.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.Mapper
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class given_a_query_provider_when_a_select_clause_is_used : SelectTests
    {
        [TestMethod]
        public void the_selected_objects_are_returned()
        {
            Actual.ShouldEqual(Expected);
        }

        public override void When()
        {
            base.When();
            Expected = new[]
                           {
                               new { One = 2, Two = 2 },
                               new { One = 4, Two = 4 },
                               new { One = 3, Two = 3 },
                               new { One = 4, Two = 4 },
                               new { One = 5, Two = 5 }
                           };
            Actual = Query.Select(item => new { One = item.IntField, Two = item.IntField }).ToArray();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class given_a_query_provider_when_two_select_clauses_are_chained : SelectTests
    {
        [TestMethod]
        public void the_selected_objects_are_returned()
        {
            Actual.ShouldEqual(Expected);
        }

        public override void When()
        {
            base.When();
            Expected = new[] { new { ABC = 2 }, new { ABC = 4 }, new { ABC = 3 }, new { ABC = 4 }, new { ABC = 5 } };
            Actual = Query.Select(item => new { One = item.IntField, Two = item.IntField })
                          .Select(item2 => new { ABC = item2.Two })
                          .ToArray();
        }
    }

    public abstract class SelectTests : QueryableContextSpecification<SimpleMockModel>
    {
        protected object Actual;

        protected object Expected;

        protected override IWorkItemStore CreateWorkItemStore()
        {
            var wis = new MockWorkItemStore().Add(
                                                  new[]
                                                      {
                                                          new Dictionary<string, object>
                                                              {
                                                                  { CoreFieldRefNames.Id, 1 },
                                                                  { CoreFieldRefNames.IterationId, 2 }
                                                              },
                                                          new Dictionary<string, object>
                                                              {
                                                                  { CoreFieldRefNames.Id, 2 },
                                                                  { CoreFieldRefNames.IterationId, 4 }
                                                              },
                                                          new Dictionary<string, object>
                                                              {
                                                                  { CoreFieldRefNames.Id, 3 },
                                                                  { CoreFieldRefNames.IterationId, 3 }
                                                              },
                                                          new Dictionary<string, object>
                                                              {
                                                                  { CoreFieldRefNames.Id, 4 },
                                                                  { CoreFieldRefNames.IterationId, 4 }
                                                              },
                                                          new Dictionary<string, object>
                                                              {
                                                                  { CoreFieldRefNames.Id, 5 },
                                                                  { CoreFieldRefNames.IterationId, 5 }
                                                              }
                                                      })
                                             .AddChildLink(3, 1)
                                             .AddChildLink(3, 2);

            return wis;
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_includes_an_empty_contains_clause : QueryableContextSpecification<SimpleMockModel>
    {
        private IEnumerable<SimpleMockModel> _actual;

        private InstrumentedMockWorkItemStore _workItemStore;

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

        public override void When()
        {
            base.When();
            var list = Enumerable.Empty<int>().ToArray();
            _actual = Query.Where(item => list.Contains(item.IntField)).ToList();
        }

        protected override IWorkItemStore CreateWorkItemStore()
        {
            _workItemStore = new InstrumentedMockWorkItemStore(base.CreateWorkItemStore());
            return _workItemStore;
        }
    }
}
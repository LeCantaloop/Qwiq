using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Core.Tests.Mocks;
using Microsoft.Qwiq.Proxies;
using Microsoft.Qwiq.Tests.Common;
using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Core.Tests
{
    [TestClass]
    public class given_an_IWorkItemStore_an_a_query_with_two_ids_and_an_asof_date : WorkItemStoreTests
    {
        [TestMethod]
        public void a_query_is_created()
        {
            QueryFactory.CreateCallCount.ShouldEqual(1);
        }

        [TestMethod]
        public void a_query_string_with_two_ids_and_an_asof_clause_is_generated()
        {
            QueryFactory.QueryWiqls.Single()
                        .ShouldEqual("SELECT * FROM WorkItems WHERE [System.Id] IN (1, 2) ASOF '2012-01-12 12:23:34Z'");
        }

        public override void Given()
        {
            QueryFactory = new MockQueryFactory();
            base.Given();
        }

        public override void When()
        {
            WorkItemStore.Query(new[] { 1, 2 }, new DateTime(2012, 1, 12, 12, 23, 34, DateTimeKind.Utc));
        }
    }

    [TestClass]
    public class given_an_IWorkItemStore_and_a_query_with_no_ids : WorkItemStoreTests
    {
        private IEnumerable<IWorkItem> _actual;

        [TestMethod]
        public void a_query_is_never_created()
        {
            QueryFactory.CreateCallCount.ShouldEqual(0);
        }

        [TestMethod]
        public void an_empty_result_set_is_returned()
        {
            _actual.ShouldBeEmpty();
        }

        public override void Given()
        {
            QueryFactory = new MockQueryFactory();
            base.Given();
        }

        public override void When()
        {
            _actual = WorkItemStore.Query(new int[] { });
        }
    }

    [TestClass]
    public class given_an_IWorkItemStore_and_a_query_with_one_id : WorkItemStoreTests
    {
        [TestMethod]
        public void a_query_is_created()
        {
            QueryFactory.CreateCallCount.ShouldEqual(1);
        }

        [TestMethod]
        public void a_query_string_with_one_id_is_generated()
        {
            QueryFactory.QueryWiqls.Single().ShouldEqual("SELECT * FROM WorkItems WHERE [System.Id] IN (1)");
        }

        public override void Given()
        {
            QueryFactory = new MockQueryFactory();
            base.Given();
        }

        public override void When()
        {
            WorkItemStore.Query(new[] { 1 });
        }
    }

    [TestClass]
    public class given_an_IWorkItemStore_and_a_query_with_two_ids : WorkItemStoreTests
    {
        [TestMethod]
        public void a_query_is_created()
        {
            QueryFactory.CreateCallCount.ShouldEqual(1);
        }

        [TestMethod]
        public void a_query_string_with_two_ids_is_generated()
        {
            QueryFactory.QueryWiqls.Single().ShouldEqual("SELECT * FROM WorkItems WHERE [System.Id] IN (1, 2)");
        }

        public override void Given()
        {
            QueryFactory = new MockQueryFactory();
            base.Given();
        }

        public override void When()
        {
            WorkItemStore.Query(new[] { 1, 2 });
        }
    }

    public abstract class WorkItemStoreTests : ContextSpecification
    {
        internal MockQueryFactory QueryFactory;

        protected IWorkItemStore WorkItemStore;

        public override void Given()
        {
            WorkItemStore = new WorkItemStoreProxy((TfsTeamProjectCollection)null, s => QueryFactory);
        }
    }
}